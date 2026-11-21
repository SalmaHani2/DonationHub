using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using MediatR;

namespace DonationHub.Application.Campaigns.Queries.GetCampaigns;

public class GetCampaignsQueryHandler
    : IRequestHandler<GetCampaignsQuery, Result<List<CampaignPublicDto>>>
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public GetCampaignsQueryHandler(
        IUnitOfWorks unitOfWork,
        TimeProvider timeProvider)
    {
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<List<CampaignPublicDto>>> Handle(
        GetCampaignsQuery request,
        CancellationToken cancellationToken)
    {
        var campaigns = await _unitOfWork.Repository<Campaign>()
            .GetAllAsync(cancellationToken);

        var result = campaigns
            .Where(c => c.Status == CampaignStatus.Active)
            .Select(c => CampaignMapper.ToPublicDto(
                c,
                request.Language,
                _timeProvider))
            .ToList();

        return Result<List<CampaignPublicDto>>.Success(result);
    }
}