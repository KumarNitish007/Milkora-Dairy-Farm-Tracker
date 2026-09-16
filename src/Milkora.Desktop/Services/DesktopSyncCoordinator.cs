using Milkora.SyncClient;

namespace Milkora.Desktop.Services;

/// <summary>
/// Owns the single <see cref="MilkoraSyncManager"/> instance for the desktop
/// process and is shared by the background hosted service (which starts it) and
/// the DesktopSyncController (which the UI calls for status / "Sync Now").
/// </summary>
public sealed class DesktopSyncCoordinator : IAsyncDisposable
{
    private readonly MilkoraSyncSettings _settings;
    private readonly ILoggerFactory _loggerFactory;
    private MilkoraSyncManager? _manager;

    public DesktopSyncCoordinator(MilkoraSyncSettings settings, ILoggerFactory loggerFactory)
    {
        _settings = settings;
        _loggerFactory = loggerFactory;
    }

    public SyncStatusInfo Status => _manager?.Status ?? new SyncStatusInfo();

    /// <summary>Creates the manager once, defaulting the sync URL to the host's own
    /// address when none is configured (so the desktop is self-contained in dev).</summary>
    public MilkoraSyncManager EnsureManager(string? selfSyncUrl)
    {
        if (_manager is not null) return _manager;

        if (string.IsNullOrWhiteSpace(_settings.ServerSyncUrl) && !string.IsNullOrWhiteSpace(selfSyncUrl))
            _settings.ServerSyncUrl = selfSyncUrl!;

        _manager = new MilkoraSyncManager(_settings, _loggerFactory.CreateLogger<MilkoraSyncManager>());
        return _manager;
    }

    public Task<SyncStatusInfo> SyncNowAsync()
        => _manager?.SynchronizeAsync() ?? Task.FromResult(Status);

    public async ValueTask DisposeAsync()
    {
        if (_manager is not null) await _manager.DisposeAsync();
    }
}
