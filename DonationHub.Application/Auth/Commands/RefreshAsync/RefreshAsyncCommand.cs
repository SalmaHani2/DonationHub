using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Auth.Commands.Refresh;

public class RefreshAsyncCommand : IRequest<Result<LoginResponse>>
{
    public RefreshRequest Request { get; set; }
    public RefreshAsyncCommand(RefreshRequest request)
    {
        Request = request;
    }
}