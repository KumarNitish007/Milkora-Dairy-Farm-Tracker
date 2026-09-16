using System.Net.NetworkInformation;
using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Milkora.SyncClient;

/// <summary>
/// Device-side sync engine. Keeps a local SQLite database in sync with the central
/// MS SQL Server via the API's Dotmim.Sync endpoint. Handles the triggers required
/// by the spec — on start, on network reconnect, on a timer, and manual "Sync Now" —
/// with retry, and raises <see cref="StatusChanged"/> for the UI indicator.
/// Conflict resolution is server-side (ServerWins), configured on the API.
/// </summary>
public sealed class MilkoraSyncManager : IAsyncDisposable
{
    private readonly MilkoraSyncSettings _settings;
    private readonly ILogger _log;
    private readonly HttpClient _http;
    private readonly bool _ownsHttp;
    private readonly SyncAgent _agent;
    private readonly SemaphoreSlim _gate = new(1, 1);

    private CancellationTokenSource? _loopCts;
    private Task? _loopTask;

    public SyncStatusInfo Status { get; private set; } = new();
    public event EventHandler<SyncStatusInfo>? StatusChanged;

    public MilkoraSyncManager(MilkoraSyncSettings settings, ILogger<MilkoraSyncManager>? logger = null, HttpClient? httpClient = null)
    {
        _settings = settings;
        _log = logger ?? NullLogger<MilkoraSyncManager>.Instance;

        if (httpClient is not null)
        {
            _http = httpClient;
            _ownsHttp = false;
        }
        else
        {
            var handler = new HttpClientHandler();
            if (settings.AcceptAnyServerCertificate)
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            _http = new HttpClient(handler);
            _ownsHttp = true;
        }

        var local = new SqliteSyncProvider(settings.LocalDbPath);
        var remote = new WebRemoteOrchestrator(settings.ServerSyncUrl, client: _http);
        _agent = new SyncAgent(local, remote);
    }

    public bool IsOnline => NetworkInterface.GetIsNetworkAvailable();

    /// <summary>Runs one synchronization (manual "Sync Now" or triggered). Serialized
    /// via a gate so overlapping triggers can't run concurrently.</summary>
    public async Task<SyncStatusInfo> SynchronizeAsync(CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct);
        try
        {
            if (!IsOnline)
                return Update(Status with { State = SyncState.Offline, Message = "No network connection." });

            Update(Status with { State = SyncState.Syncing, Message = "Syncing…", Error = null });

            Exception? last = null;
            var attempts = Math.Max(1, _settings.MaxRetries);
            for (var attempt = 1; attempt <= attempts; attempt++)
            {
                try
                {
                    var result = await _agent.SynchronizeAsync(
                        SyncOptions.DefaultScopeName, null, SyncType.Normal, null, null, ct);
                    _log.LogInformation("Sync ok: ↑{Up} ↓{Down} conflicts {Conf}",
                        result.TotalChangesUploadedToServer, result.TotalChangesDownloadedFromServer, result.TotalResolvedConflicts);

                    return Update(Status with
                    {
                        State = SyncState.Synced,
                        Message = "Up to date.",
                        LastSyncedAt = DateTime.Now,
                        ChangesUploaded = result.TotalChangesUploadedToServer,
                        ChangesDownloaded = result.TotalChangesDownloadedFromServer,
                        TotalConflicts = result.TotalResolvedConflicts,
                        Error = null,
                    });
                }
                catch (Exception ex) when (attempt < attempts && !ct.IsCancellationRequested)
                {
                    last = ex;
                    _log.LogWarning(ex, "Sync attempt {Attempt}/{Total} failed; retrying in {Delay}s.", attempt, attempts, _settings.RetryDelaySeconds);
                    await Task.Delay(TimeSpan.FromSeconds(_settings.RetryDelaySeconds), ct);
                }
            }

            return Update(Status with { State = SyncState.Error, Message = "Sync failed.", Error = last?.Message });
        }
        catch (OperationCanceledException)
        {
            return Status;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected sync failure.");
            return Update(Status with { State = SyncState.Error, Message = "Sync failed.", Error = ex.Message });
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>Starts background sync: an immediate sync, then on a timer and on
    /// network reconnect.</summary>
    public async Task StartAsync(CancellationToken ct = default)
    {
        NetworkChange.NetworkAvailabilityChanged += OnNetworkChanged;
        await SynchronizeAsync(ct);

        if (_settings.SyncIntervalMinutes > 0)
        {
            _loopCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            _loopTask = RunLoopAsync(_loopCts.Token);
        }
    }

    private async Task RunLoopAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_settings.SyncIntervalMinutes));
        try
        {
            while (await timer.WaitForNextTickAsync(ct))
                await SynchronizeAsync(ct);
        }
        catch (OperationCanceledException) { /* stopping */ }
    }

    private async void OnNetworkChanged(object? sender, NetworkAvailabilityEventArgs e)
    {
        if (e.IsAvailable)
        {
            try { await SynchronizeAsync(); } catch { /* status already reflects error */ }
        }
        else
        {
            Update(Status with { State = SyncState.Offline, Message = "Offline." });
        }
    }

    public async Task StopAsync()
    {
        NetworkChange.NetworkAvailabilityChanged -= OnNetworkChanged;
        _loopCts?.Cancel();
        if (_loopTask is not null)
        {
            try { await _loopTask; } catch { /* ignore */ }
        }
    }

    private SyncStatusInfo Update(SyncStatusInfo status)
    {
        Status = status;
        StatusChanged?.Invoke(this, status);
        return status;
    }

    public async ValueTask DisposeAsync()
    {
        await StopAsync();
        if (_ownsHttp) _http.Dispose();
    }
}
