using DonationHub.Application.Common.Results;
using MediatR;

namespace DonationHub.Application.Campaigns.Commands.DeleteCampaign;

public record DeleteCampaignCommand(Guid Id) : IRequest<Result>;