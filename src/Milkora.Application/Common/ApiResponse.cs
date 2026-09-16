namespace Milkora.Application.Common;

/// <summary>Uniform response envelope for every endpoint (success and failure).</summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IReadOnlyDictionary<string, string[]>? Errors { get; set; }

    public static ApiResponse Ok(string? message = null) => new() { Success = true, Message = message };

    public static ApiResponse Fail(string message, IReadOnlyDictionary<string, string[]>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}

/// <summary>Envelope carrying a data payload.</summary>
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
        => new() { Success = true, Data = data, Message = message };

    public static new ApiResponse<T> Fail(string message, IReadOnlyDictionary<string, string[]>? errors = null)
        => new() { Success = false, Message = message, Errors = errors };
}
