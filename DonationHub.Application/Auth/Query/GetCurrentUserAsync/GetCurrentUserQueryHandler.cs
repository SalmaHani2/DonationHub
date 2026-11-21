using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Identity;
using MediatR;

namespace DonationHub.Application.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<UserProfileResponse>>
{
    private readonly IIdentityService _identityService;

    public GetCurrentUserQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<UserProfileResponse>> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _identityService.FindByIdAsync(
            request.UserId, cancellationToken);


        var response = new UserProfileResponse
        {
            Email = user.Email,
            FullName = user.FullName,
            PreferredLanguage = user.PreferredLanguage,
            Roles = user.Roles
        };

        return Result<UserProfileResponse>.Success(response);
    }
}