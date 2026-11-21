using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Auth.Commands.Register;

public class RegisterAsyncCommand : IRequest<Result<RegisterResponse>>
{
    public RegisterRequest Request { get; set; }
}