using Microsoft.AspNetCore.Mvc;
using Milkora.Desktop.Services;

namespace Milkora.Desktop.Controllers;

/// <summary>
/// Bridges the Angular UI to the device sync engine (desktop build only).
/// Returns the ApiResponse-shaped envelope the client already understands.
/// In the web build these routes don't exist; the client falls back gracefully.
/// </summary>
[ApiController]
[Route("api/desktop/sync")]
public sealed class DesktopSyncController : ControllerBase
{
    private readonly DesktopSyncCoordinator _coordinator;
    public DesktopSyncController(DesktopSyncCoordinator coordinator) => _coordinator = coordinator;

    // GET api/desktop/sync/status
    [HttpGet("status")]
    public IActionResult Status()
        => Ok(new { success = true, data = _coordinator.Status });

    // POST api/desktop/sync/now  — manual "Sync Now"
    [HttpPost("now")]
    public async Task<IActionResult> Now()
        => Ok(new { success = true, data = await _coordinator.SyncNowAsync() });
}
