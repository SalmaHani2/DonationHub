using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Auth.Commands.Login;

public class LoginAsyncCommand : IRequest<Result<LoginResponse>>
{
   public LoginRequest Request { get; set; }
    public LoginAsyncCommand(LoginRequest request)
    {
        Request = request;
    }
}