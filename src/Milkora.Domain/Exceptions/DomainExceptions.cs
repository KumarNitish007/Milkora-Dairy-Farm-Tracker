namespace Milkora.Domain.Exceptions;

/// <summary>Base for all expected/handled application errors. The API's
/// exception middleware maps each derived type to an HTTP status code.</summary>
public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }
    protected AppException(string message) : base(message) { }
}

/// <summary>404 - a requested resource does not exist.</summary>
public sealed class NotFoundException : AppException
{
    public override int StatusCode => 404;
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string resource, object key)
        : base($"{resource} with id '{key}' was not found.") { }
}

/// <summary>400 - the request failed business/input validation.
/// Carries per-field errors for the response envelope.</summary>
public sealed class ValidationException : AppException
{
    public override int StatusCode => 400;
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("One or more validation errors occurred.")
        => Errors = errors;

    public ValidationException(string field, string error)
        : this(new Dictionary<string, string[]> { [field] = new[] { error } }) { }
}

/// <summary>409 - the request conflicts with current state (e.g. duplicate tag).</summary>
public sealed class ConflictException : AppException
{
    public override int StatusCode => 409;
    public ConflictException(string message) : base(message) { }
}
