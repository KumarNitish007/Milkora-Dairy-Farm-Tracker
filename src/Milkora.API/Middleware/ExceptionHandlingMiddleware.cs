using System.Text.Json;
using Milkora.Application.Common;
using Milkora.Domain.Exceptions;

namespace Milkora.API.Middleware;

/// <summary>
/// Single place that turns any unhandled exception into a consistent
/// ApiResponse envelope with the right HTTP status code. Known AppExceptions
/// map to their StatusCode; everything else becomes a logged 500.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteAsync(context, ex.StatusCode, ApiResponse.Fail(ex.Message, ex.Errors));
        }
        catch (AppException ex)
        {
            // Expected business errors (404 / 409) — info level, no stack trace noise.
            _logger.LogInformation("Handled {Type}: {Message}", ex.GetType().Name, ex.Message);
            await WriteAsync(context, ex.StatusCode, ApiResponse.Fail(ex.Message));
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // Client disconnected — nothing to return.
            _logger.LogDebug("Request aborted by the client.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing {Path}", context.Request.Path);
            await WriteAsync(context, StatusCodes.Status500InternalServerError,
                ApiResponse.Fail("An unexpected error occurred."));
        }
    }

    private static async Task WriteAsync(HttpContext context, int statusCode, ApiResponse body)
    {
        if (context.Response.HasStarted) return;
        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
}
