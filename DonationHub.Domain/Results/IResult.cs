using DonationHub.Application.Common.Errors;

namespace DonationHub.Application.Common.Results;

public interface IResult<TSelf>
    where TSelf : IResult<TSelf>
{
    static abstract TSelf Failure(
        Error error,
        Dictionary<string, string[]>? validationErrors = null
    );
}