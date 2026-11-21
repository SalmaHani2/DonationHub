using DonationHub.Application.Common;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Application.Categories.Commands.DeleteAsync
{
    internal class DeleteAsyncHandler : IRequestHandler<DeleteAsyncCommand, Result<bool>>
    {
        private readonly IUnitOfWorks _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public DeleteAsyncHandler(IUnitOfWorks unitOfWork, TimeProvider timeProvider)
        {
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<bool>> Handle(DeleteAsyncCommand request, CancellationToken cancellationToken)
        {
            var repository = _unitOfWork.Repository<Category>();

            var category = await repository.GetByIdWithIncludesAsync(
                request.Id,
                cancellationToken,
                category => category.Campaigns);

        

           
                category.EnsureCanBeDeleted();
            

            category.Deleted(_timeProvider.GetUtcNow().UtcDateTime);

            await repository.UpdateAsync(category, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
