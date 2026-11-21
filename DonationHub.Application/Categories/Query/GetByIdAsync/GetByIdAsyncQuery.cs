using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Categories.Query.GetByIdAsync
{
    public class GetByIdAsyncQuery : IRequest<Result<CategoryResponse>>
    {
        public Guid Id { get; set; }
    }
}
