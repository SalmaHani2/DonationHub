using FluentValidation;

namespace DonationHub.Application.Campaigns.Commands.CreateCampaign;

public class CreateCampaignCommandValidator
    : AbstractValidator<CreateCampaignCommand>
{
    public CreateCampaignCommandValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Campaign.TitleAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Campaign.TitleEn)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Campaign.DescriptionAr)
            .NotEmpty();

        RuleFor(x => x.Campaign.DescriptionEn)
            .NotEmpty();

        RuleFor(x => x.Campaign.GoalAmount)
            .GreaterThan(0)
            .WithErrorCode("Validation.Failed");

        RuleFor(x => x.Campaign.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Campaign.EndDate)
            .Must(date => date > timeProvider.GetUtcNow().UtcDateTime)
            .WithErrorCode("Campaign.EndDateInPast");
    }
}