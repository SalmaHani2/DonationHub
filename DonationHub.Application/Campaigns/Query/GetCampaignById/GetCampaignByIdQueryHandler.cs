using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using MediatR;

namespace DonationHub.Application.Campaigns.Queries.GetCampaignById;

public class GetCampaignByIdQueryHandler
    : IRequestHandler<GetCampaignByIdQuery, Result<CampaignPublicDto>>
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public GetCampaignByIdQueryHandler(
        IUnitOfWorks unitOfWork,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<CampaignPublicDto>> Handle(
        GetCampaignByIdQuery request,
        CancellationToken cancellationToken)
    {
        var campaign = await _unitOfWork.Repository<Campaign>()
            .GetByIdAsync(request.Id, cancellationToken);

        if (campaign == null)
        {
            return Result<CampaignPublicDto>.Failure(
                ErrorCatalog.Campaign.NotFound);
        }

        var result = CampaignMapper.ToPublicDto(
            campaign,
            request.Language,
            _timeProvider);

        return Result<CampaignPublicDto>.Success(result);
    }
}