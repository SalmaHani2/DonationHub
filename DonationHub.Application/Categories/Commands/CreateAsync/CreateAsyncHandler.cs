using DonationHub.Application.Categories.Categories;
using DonationHub.Application.Categories.Commands.CreateAsync;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler
    : IRequestHandler<CreateCategoryCommand, Result<CategoryResponse>>
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CreateCategoryCommandHandler(IUnitOfWorks unitOfWork, TimeProvider timeProvider)
    {
        
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<CategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var currentTime = _timeProvider.GetUtcNow().UtcDateTime;

        var category = Category.Create(
            request.Request.NameAr,
            request.Request.NameEn,
            request.Request.DescriptionAr,
            request.Request.DescriptionEn,
            currentTime);

        await _unitOfWork.Repository<Category>()
            .AddAsync(category, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var response = new CategoryResponse
        {
            Id = category.Id,
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn
        };

        return Result<CategoryResponse>.Success(response);
    }
}