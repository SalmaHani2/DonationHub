using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Campaigns.Commands.CreateCampaign;

public record CreateCampaignCommand(
    CreateCampaignDto Campaign,
    Guid CreatedByUserId
) : IRequest<Result<CampaignAdminDto>>;