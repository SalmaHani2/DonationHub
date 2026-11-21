namespace DonationHub.Application.Common.Errors;

public static class ErrorCatalog
{
    public static class Validation
    {
        public static readonly Error Failed = new(
            "Validation.Failed",
            "One or more validation errors occurred.",
            ErrorType.Validation
        );
    }

    public static class Auth
    {
        public static readonly Error InvalidCredentials = new(
            "Auth.InvalidCredentials",
            "The email or password is incorrect.",
            ErrorType.Unauthorized
        );

        public static readonly Error EmailAlreadyExists = new(
            "Auth.EmailAlreadyExists",
            "An account with this email already exists.",
            ErrorType.Conflict
        );

        public static readonly Error RefreshTokenInvalid = new(
            "Auth.RefreshTokenInvalid",
            "The refresh token is invalid, expired, or revoked.",
            ErrorType.Unauthorized
        );

        public static readonly Error AccountDeactivated = new(
            "Auth.AccountDeactivated",
            "This account is deactivated.",
            ErrorType.Forbidden
        );
    }

    public static class Category
    {
        public static readonly Error NotFound = new(
            "Category.NotFound",
            "The specified category was not found.",
            ErrorType.NotFound
        );

        public static readonly Error HasCampaigns = new(
            "Category.HasCampaigns",
            "Cannot delete a category with linked campaigns.",
            ErrorType.Conflict
        );
    }



    public static class Donation
    {
        public static readonly Error NotFound = new(
            "Donation.NotFound",
            "Donation was not found or is not owned by the user.",
            ErrorType.NotFound
        );

        public static readonly Error AmountOutOfRange = new(
            "Donation.AmountOutOfRange",
            "Donation amount is outside the allowed range.",
            ErrorType.UnprocessableEntity
        );

        public static readonly Error InvalidState = new(
            "Donation.InvalidState",
            "Donation is already in a final state.",
            ErrorType.Conflict
        );
    }

    public static class Common
    {
        public static readonly Error RateLimited = new(
            "Common.RateLimited",
            "Rate limit exceeded.",
            ErrorType.TooManyRequests
        );
    }

    public static class File
    {
        public static readonly Error InvalidType = new(
            "File.InvalidType",
            "File signature is not allowed.",
            ErrorType.Validation
        );

        public static readonly Error TooLarge = new(
            "File.TooLarge",
            "File exceeds the maximum allowed size.",
            ErrorType.PayloadTooLarge
        );
    }

    public static class Payment
    {
        public static readonly Error InvalidSignature = new(
            "Payment.InvalidSignature",
            "Stripe webhook signature is invalid.",
            ErrorType.Validation
        );
    }

    public static class Idempotency
    {
        public static readonly Error KeyReusedWithDifferentRequest = new(
            "Idempotency.KeyReusedWithDifferentRequest",
            "The same Idempotency-Key was used with a different request body.",
            ErrorType.UnprocessableEntity
        );
    }
    /// <summary>
    /// ////////////////////////////
    public static class Campaign
    {
        public static readonly Error NotFound = new(
            "Campaign.NotFound",
            "Campaign not found.",
            ErrorType.NotFound);

        public static readonly Error NotActive = new(
            "Campaign.NotActive",
            "Campaign is not active.",
            ErrorType.Conflict);

        public static readonly Error GoalBelowRaised = new(
            "Campaign.GoalBelowRaised",
            "Goal amount cannot be less than raised amount.",
            ErrorType.Validation);

        public static readonly Error EndDateInPast = new(
            "Campaign.EndDateInPast",
            "Campaign end date must be in the future.",
            ErrorType.Validation);

        public static readonly Error ConcurrencyConflict = new(
            "Campaign.ConcurrencyConflict",
            "Campaign was modified by another user.",
            ErrorType.Conflict);
    }

    ///////////////////////////////////////
    public static readonly Error RateLimited = new(
       "Common.RateLimited",
       "Rate limit exceeded.",
       ErrorType.TooManyRequests
   );

    public static readonly Error InternalError = new(
        "Common.InternalError",
        "An unexpected error occurred. Please try again later.",
        ErrorType.Failure
    );
}
