
using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Common;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Campaigns.Commands.CreateCampaign;

public class CreateCampaignCommandHandler
    : IRequestHandler<CreateCampaignCommand, Result<CampaignAdminDto>>
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly TimeProvider _timeProvider;

    private const string DefaultCurrency = "USD";

    public CreateCampaignCommandHandler(
        IUnitOfWorks unitOfWork,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<CampaignAdminDto>> Handle(
        CreateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Campaign;

        var category = await _unitOfWork.Repository<Category>()
            .GetByIdAsync(dto.CategoryId, cancellationToken);

        if (category == null)
        {
            return Result<CampaignAdminDto>.Failure(
                ErrorCatalog.Category.NotFound);
        }

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var campaignResult = Campaign.Create(
            Money.Create(dto.GoalAmount, DefaultCurrency),
            dto.EndDate,
            now);

        if (!campaignResult.IsSuccess)
        {
            return Result<CampaignAdminDto>.Failure(
                campaignResult.Error!);
        }

        var campaign = campaignResult.Value!;

        await _unitOfWork.Repository<Campaign>()
            .AddAsync(campaign, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<CampaignAdminDto>.Success(
            CampaignMapper.ToAdminDto(campaign));
    }
}