namespace LitXusTravel.Application.Common.Models;

public class Result
{
    public bool IsSuccess { get; init; }
    public string[] Errors { get; init; } = [];

    public static Result Success() => new() { IsSuccess = true };
    public static Result Failure(params string[] errors) => new() { IsSuccess = false, Errors = errors };
    public static Result Failure(IEnumerable<string> errors) => new() { IsSuccess = false, Errors = errors.ToArray() };
}

public class Result<T> : Result
{
    public T? Value { get; init; }

    public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
    public new static Result<T> Failure(params string[] errors) => new() { IsSuccess = false, Errors = errors };
    public new static Result<T> Failure(IEnumerable<string> errors) => new() { IsSuccess = false, Errors = errors.ToArray() };
}
