using DonationHub.Application.Common.Errors;

namespace DonationHub.Application.Common.Results;

public class Result : IResult<Result>
{
    public bool IsSuccess { get; private set; }

    public Error? Error { get; private set; }

    public Dictionary<string, string[]>? ValidationErrors
    { get; private set; }

    private Result(
        bool isSuccess,
        Error? error,
        Dictionary<string, string[]>? validationErrors)
    {
        IsSuccess = isSuccess;
        Error = error;
        ValidationErrors = validationErrors;
    }

    public static Result Success()
    {
        return new Result(true, null, null);
    }

    public static Result Failure(
        Error error,
        Dictionary<string, string[]>? validationErrors = null)
    {
        return new Result(false, error, validationErrors);
    }
}