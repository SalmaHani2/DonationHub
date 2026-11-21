
using FluentValidation;

namespace DonationHub.Application.Campaigns.Commands.DeleteCampaign;

public class DeleteCampaignCommandValidator
    : AbstractValidator<DeleteCampaignCommand>
{
    public DeleteCampaignCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Campaign ID is required.");
    }
}
