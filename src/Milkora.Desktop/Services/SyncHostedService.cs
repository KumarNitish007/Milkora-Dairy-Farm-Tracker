using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Milkora.SyncClient;

namespace Milkora.Desktop.Services;

/// <summary>Starts the background sync engine once Kestrel is listening,
/// resolving the host's own address as the default sync target.</summary>
public sealed class SyncHostedService : IHostedService
{
    private readonly DesktopSyncCoordinator _coordinator;
    private readonly IServer _server;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ILogger<SyncHostedService> _log;

    public SyncHostedService(DesktopSyncCoordinator coordinator, IServer server, IHostApplicationLifetime lifetime, ILogger<SyncHostedService> log)
    {
        _coordinator = coordinator;
        _server = server;
        _lifetime = lifetime;
        _log = log;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Defer until the server has bound its addresses.
        _lifetime.ApplicationStarted.Register(() => _ = InitializeAsync());
        return Task.CompletedTask;
    }

    private async Task InitializeAsync()
    {
        var addresses = _server.Features.Get<IServerAddressesFeature>()?.Addresses;
        var self = addresses?.FirstOrDefault(a => a.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                   ?? addresses?.FirstOrDefault();
        var selfSyncUrl = self is null ? null : self.TrimEnd('/') + "/api/sync";

        _log.LogInformation("Starting background sync engine (target: {Url}).", selfSyncUrl ?? "configured");
        var manager = _coordinator.EnsureManager(selfSyncUrl);

        try
        {
            await manager.StartAsync(_lifetime.ApplicationStopping);
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Background sync engine failed to start.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
