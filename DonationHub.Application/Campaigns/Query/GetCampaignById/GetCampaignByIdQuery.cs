using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Campaigns.Queries.GetCampaignById;

public record GetCampaignByIdQuery(Guid Id, string Language)
    : IRequest<Result<CampaignPublicDto>>;