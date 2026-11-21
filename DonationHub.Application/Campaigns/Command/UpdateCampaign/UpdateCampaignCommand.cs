using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Campaigns.Commands.UpdateCampaign;

public record UpdateCampaignCommand(
    Guid Id,
    UpdateCampaignDto Campaign
) : IRequest<Result<CampaignAdminDto>>;