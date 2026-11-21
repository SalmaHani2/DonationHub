using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Campaigns.Queries.GetCampaigns;

public record GetCampaignsQuery(string Language)
    : IRequest<Result<List<CampaignPublicDto>>>;