using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Categories.Query.GetByIdAsync
{
    internal class GetByIdAsyncHandler : IRequestHandler<GetByIdAsyncQuery, Result<CategoryResponse>>
    {
        private readonly IUnitOfWorks _unitOfWork;

        public GetByIdAsyncHandler(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryResponse>> Handle(
            GetByIdAsyncQuery request,
            CancellationToken cancellationToken)
        {
            var category = await _unitOfWork
                .Repository<Category>()
                .GetByIdAsync(request.Id, cancellationToken);


            var response = new CategoryResponse
            {
                Id = category.Id,
                NameAr = category.NameAr!,
                NameEn = category.NameEn!,
                DescriptionAr = category.DescriptionAr,
                DescriptionEn = category.DescriptionEn
            };

            return Result<CategoryResponse>.Success(response);
        }
    }
}
