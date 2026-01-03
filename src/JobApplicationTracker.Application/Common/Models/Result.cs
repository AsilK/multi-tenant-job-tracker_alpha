namespace JobApplicationTracker.Application.Common.Models;

/// <summary>
/// Represents the result of an operation that can succeed or fail.
/// </summary>
/// <typeparam name="T">The type of data returned on success.</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public IDictionary<string, string[]>? ValidationErrors { get; private set; }

    private Result() { }

    /// <summary>
    /// Creates a successful result with data.
    /// </summary>
    public static Result<T> Success(T data) => new()
    {
        IsSuccess = true,
        Data = data
    };

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result<T> Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };

    /// <summary>
    /// Creates a failed result with validation errors.
    /// </summary>
    public static Result<T> ValidationFailure(IDictionary<string, string[]> errors) => new()
    {
        IsSuccess = false,
        Error = "Validation failed",
        ValidationErrors = errors
    };
}

/// <summary>
/// Represents the result of an operation without return data.
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }

    private Result() { }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new() { IsSuccess = true };

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static Result Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}
