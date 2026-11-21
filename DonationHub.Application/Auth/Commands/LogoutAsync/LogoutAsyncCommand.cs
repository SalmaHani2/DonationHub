using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Auth.Commands.Logout;

public class LogoutAsyncCommand : IRequest<Result<bool>>
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}