using DonationHub.Application.Admin.Admin;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Admin.Commands.CreateStaff;

public class CreateStaffAsyncCommand : IRequest<Result<CreateStaffResponse>>
{
   public CreateStaffRequest Request { get; set; }
    public CreateStaffAsyncCommand(CreateStaffRequest request)
    {
        Request = request;
    }
}