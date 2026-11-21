
using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Identity;
using MediatR;

namespace DonationHub.Application.Auth.Commands.Register;

public class RegisterAsyncHandler
    : IRequestHandler<RegisterAsyncCommand, Result<RegisterResponse>>
{
    private readonly IIdentityService _identityService;

    public RegisterAsyncHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<RegisterResponse>> Handle(
        RegisterAsyncCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim();

        var result = await _identityService.CreateUserAsync(
            email,
            request.Request.FullName.Trim(),
            request.Request.PreferredLanguage.Trim().ToLowerInvariant(),
            request.Request.Password,
            cancellationToken);

        if (!result.IsSuccess)
            return Result<RegisterResponse>.Failure(result.Error!);

        var user = result.Value;

        var response = new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PreferredLanguage = user.PreferredLanguage
        };

        return Result<RegisterResponse>.Success(response);
    }
}
