using DonationHub.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Categories.Commands.DeleteAsync
{
    public class DeleteAsyncCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
    }
}
    