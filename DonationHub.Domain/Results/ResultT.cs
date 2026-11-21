using DonationHub.Application.Common.Errors;

namespace DonationHub.Application.Common.Results;

public class Result<T> : IResult<Result<T>>
{
    public bool IsSuccess { get; private set; }

    public T? Value { get; private set; }

    public Error? Error { get; private set; }

    public Dictionary<string, string[]>? ValidationErrors
    { get; private set; }

    private Result(
        bool isSuccess,
        T? value,
        Error? error,
        Dictionary<string, string[]>? validationErrors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, null, null);
    }

    public static Result<T> Failure(
        Error error,
        Dictionary<string, string[]>? validationErrors = null)
    {
        return new Result<T>(false, default, error, validationErrors);
    }
}