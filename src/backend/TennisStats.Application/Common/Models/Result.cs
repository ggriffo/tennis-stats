namespace TennisStats.Application.Common.Models;

/// <summary>
/// Standard API result wrapper
/// </summary>
/// <typeparam name="T">Type of data</typeparam>
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, T? data, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result<T> Success(T data) => new(true, data, null, null);
    public static Result<T> Failure(string error, string? errorCode = null) => new(false, default, error, errorCode);
}

/// <summary>
/// Non-generic result for operations without return data
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public string? Error { get; }
    public string? ErrorCode { get; }

    private Result(bool isSuccess, string? error, string? errorCode)
    {
        IsSuccess = isSuccess;
        Error = error;
        ErrorCode = errorCode;
    }

    public static Result Success() => new(true, null, null);
    public static Result Failure(string error, string? errorCode = null) => new(false, error, errorCode);
}
