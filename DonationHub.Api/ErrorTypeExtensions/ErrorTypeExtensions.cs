using DonationHub.Application.Common.Errors;

public static class ErrorTypeExtensions
{
    public static int ToStatusCode(this ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => 400,
            ErrorType.Unauthorized => 401,
            ErrorType.Forbidden => 403,
            ErrorType.NotFound => 404,
            ErrorType.Conflict => 409,
            ErrorType.PayloadTooLarge => 413,
            ErrorType.UnprocessableEntity => 422,
            ErrorType.TooManyRequests => 429,
            ErrorType.Failure => 500,
            _ => 500
        };
    }
}