using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Campaigns.Commands.UpdateCampaign;

public class UpdateCampaignCommandHandler
    : IRequestHandler<UpdateCampaignCommand, Result<CampaignAdminDto>>
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly TimeProvider _timeProvider;

    private const string DefaultCurrency = "USD";

    public UpdateCampaignCommandHandler(
        IUnitOfWorks unitOfWork,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<CampaignAdminDto>> Handle(
        UpdateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var repository = _unitOfWork.Repository<Campaign>();

        var campaign = await repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (campaign == null)
        {
            return Result<CampaignAdminDto>.Failure(
                ErrorCatalog.Campaign.NotFound);
        }

        var dto = request.Campaign;

        byte[] rowVersion;

        try
        {
            rowVersion = Convert.FromBase64String(dto.RowVersion);
        }
        catch (FormatException)
        {
            return Result<CampaignAdminDto>.Failure(
                ErrorCatalog.Validation.Failed);
        }

        repository.SetOriginalRowVersion(campaign, rowVersion);

        var category = await _unitOfWork.Repository<Category>()
            .GetByIdAsync(dto.CategoryId, cancellationToken);

        if (category == null)
        {
            return Result<CampaignAdminDto>.Failure(
                ErrorCatalog.Category.NotFound);
        }

       
            campaign.Update(
                dto.TitleAr,
                dto.TitleEn,
                dto.DescriptionAr,
                dto.DescriptionEn,
                Money.Create(dto.GoalAmount, DefaultCurrency),
                dto.EndDate,
                dto.CategoryId,
                _timeProvider.GetUtcNow().UtcDateTime);
        
      

        await repository.UpdateAsync(campaign, cancellationToken);

        var saved = await _unitOfWork.TrySaveChangesAsync(cancellationToken);

        if (!saved)
        {
            return Result<CampaignAdminDto>.Failure(
                ErrorCatalog.Campaign.ConcurrencyConflict);
        }

        return Result<CampaignAdminDto>.Success(
            CampaignMapper.ToAdminDto(campaign));
    }
}