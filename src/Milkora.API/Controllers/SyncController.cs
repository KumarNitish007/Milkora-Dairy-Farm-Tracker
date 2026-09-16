using Dotmim.Sync.Web.Server;
using Microsoft.AspNetCore.Mvc;

namespace Milkora.API.Controllers;

/// <summary>
/// Dotmim.Sync server endpoint. The SQLite clients POST their batches here; the
/// injected <see cref="WebServerAgent"/> handles change tracking, upload/download
/// and conflict resolution (ServerWins) against the central MS SQL Server.
///
/// This controller is deliberately NOT built on ApiControllerBase: the sync
/// protocol writes its own binary/JSON payload straight to the response, so it
/// must bypass the ApiResponse envelope. Because the agent starts the response,
/// the global exception middleware leaves it untouched.
/// </summary>
[ApiController]
[Route("api/sync")]
public sealed class SyncController : ControllerBase
{
    private readonly WebServerAgent _agent;

    public SyncController(WebServerAgent agent) => _agent = agent;

    // Client sync traffic.
    [HttpPost]
    public Task Post() => _agent.HandleRequestAsync(HttpContext);

    // Human-friendly status page (Development aid): shows the configured scope.
    [HttpGet]
    public Task Get() => HttpContext.WriteHelloAsync(_agent);
}
