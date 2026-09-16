using Microsoft.AspNetCore.Mvc;
using Milkora.Application.Common;

namespace Milkora.API.Controllers;

/// <summary>Base for all API controllers: helpers that wrap results in the
/// standard ApiResponse envelope so controllers stay one-liners.</summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult OkData<T>(T data, string? message = null)
        => Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult Created<T>(T data, string? message = null)
        => StatusCode(StatusCodes.Status201Created, ApiResponse<T>.Ok(data, message));

    protected IActionResult OkMessage(string message)
        => Ok(ApiResponse.Ok(message));
}
