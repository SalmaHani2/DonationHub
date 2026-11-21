using DonationHub.Application.Categories.Categories;
using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Categories.Commands.CreateAsync
{
    public class CreateCategoryCommand : IRequest<Result<CategoryResponse>>
    {
        public CreateCategoryRequest Request { get; set; } = null!;
    }
}
