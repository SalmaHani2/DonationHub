using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Categories.Query.GetAllAsync
{
    internal class GetAllAsyncHnadler : IRequestHandler<GetAllAsyncQuery, Result<List<CategoryResponse>>>
    {
        private readonly IUnitOfWorks _unitOfWork;

        public GetAllAsyncHnadler(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<List<CategoryResponse>>> Handle(
            GetAllAsyncQuery request,
            CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork
                .Repository<Category>()
                .GetAllAsync(cancellationToken);

            var response = categories.Select(category => new CategoryResponse
            {
                Id = category.Id,
                NameAr = category.NameAr!,
                NameEn = category.NameEn!,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn
            }).ToList();

            return Result<List<CategoryResponse>>.Success(response);
        }
    }
}
    
