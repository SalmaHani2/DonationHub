using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Categories.Commands.UpdateAsync
{
    internal class UpdateAsyncHandler : IRequestHandler<UpdateAsyncCommand, Result<CategoryResponse>>
    {
        private readonly IUnitOfWorks _unitOfWork;

        public UpdateAsyncHandler(IUnitOfWorks unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CategoryResponse>> Handle(UpdateAsyncCommand request, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Category>();

            var category = await repository.GetByIdAsync(
                request.Id, cancellationToken);

          

            var categories = await repository.GetAllAsync(cancellationToken);

            var exists = categories.Any(c =>
                c.Id != request.Id &&
                (c.NameAr == request.Request.NameAr.Trim() ||
                 c.NameEn == request.Request.NameEn.Trim()));

          

            category.Update(
                request.Request.NameAr.Trim(),
                request.Request.NameEn.Trim(),
                request.Request.DescriptionAr?.Trim(),
                request.Request.DescriptionEn?.Trim(),
                DateTime.UtcNow);

            await repository.UpdateAsync(category, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

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