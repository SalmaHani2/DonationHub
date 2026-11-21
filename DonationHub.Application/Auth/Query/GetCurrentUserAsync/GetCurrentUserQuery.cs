using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<Result<UserProfileResponse>>
{
    public Guid UserId { get; set; }
}