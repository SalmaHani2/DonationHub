using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Common.Results;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Categories.Query.GetAllAsync
{
    public class GetAllAsyncQuery : IRequest<Result<List<CategoryResponse>>>
    {
    }
}
