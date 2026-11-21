using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace DonationHub.Application.Categories.Commands.UpdateAsync
{
    public class UpdateAsyncCommand : IRequest<Result<CategoryResponse>>
    {
        public Guid Id { get; set; }
        public UpdateCategoryRequest Request { get; set; } = null!;
    }
}
