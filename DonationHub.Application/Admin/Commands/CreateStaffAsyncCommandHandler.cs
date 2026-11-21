using DonationHub.Application.Admin.Admin;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Identity;
using MediatR;

namespace DonationHub.Application.Admin.Commands.CreateStaff;

public class CreateStaffِAsyncCommandHandler
    : IRequestHandler<CreateStaffAsyncCommand, Result<CreateStaffResponse>>
{
    private readonly IIdentityService _identityService;

    public CreateStaffِAsyncCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<CreateStaffResponse>> Handle(CreateStaffAsyncCommand request,CancellationToken cancellationToken)
    {
        var email = request.Request.Email.Trim();

        var createResult = await _identityService.CreateUserAsync(
            email,
            request.Request.FullName.Trim(),
            "en",
            request.Request.TemporaryPassword,
            cancellationToken);
     
        var roleResult = await _identityService.AddToRoleAsync(
            email,
            request.Request.Role,
            cancellationToken);


        return Result<CreateStaffResponse>.Success(
            new CreateStaffResponse
            {
                Email = email,
                Role = request.Request.Role
            });
    }
}