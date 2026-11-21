namespace DonationHub.Application.Common.Errors;

public enum ErrorType
{
    Validation,
    Unauthorized,
    Forbidden,
    NotFound,
    Conflict,
    PayloadTooLarge,
    UnprocessableEntity,
    TooManyRequests,
    Failure
}
