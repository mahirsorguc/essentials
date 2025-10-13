namespace HMS.Essentials.MediatR;

/// <summary>
/// Represents the result of an operation.
/// </summary>
/// <typeparam name="T">The type of the result value.</typeparam>
public class Result<T>
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the result value.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets the collection of errors if the operation failed.
    /// </summary>
    public IReadOnlyCollection<string>? Errors { get; }

    private Result(bool isSuccess, T? value, string? error, IReadOnlyCollection<string>? errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        Errors = errors;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result<T> Success(T value) => new(true, value, null, null);

    /// <summary>
    /// Creates a failed result with a single error message.
    /// </summary>
    public static Result<T> Failure(string error) => new(false, default, error, null);

    /// <summary>
    /// Creates a failed result with multiple error messages.
    /// </summary>
    public static Result<T> Failure(IReadOnlyCollection<string> errors) => new(false, default, null, errors);

    /// <summary>
    /// Creates a failed result with multiple error messages.
    /// </summary>
    public static Result<T> Failure(params string[] errors) => new(false, default, null, errors);
}

/// <summary>
/// Represents the result of an operation without a value.
/// </summary>
public class Result
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Gets the collection of errors if the operation failed.
    /// </summary>
    public IReadOnlyCollection<string>? Errors { get; }

    private Result(bool isSuccess, string? error, IReadOnlyCollection<string>? errors)
    {
        IsSuccess = isSuccess;
        Error = error;
        Errors = errors;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, null, null);

    /// <summary>
    /// Creates a failed result with a single error message.
    /// </summary>
    public static Result Failure(string error) => new(false, error, null);

    /// <summary>
    /// Creates a failed result with multiple error messages.
    /// </summary>
    public static Result Failure(IReadOnlyCollection<string> errors) => new(false, null, errors);

    /// <summary>
    /// Creates a failed result with multiple error messages.
    /// </summary>
    public static Result Failure(params string[] errors) => new(false, null, errors);
}
