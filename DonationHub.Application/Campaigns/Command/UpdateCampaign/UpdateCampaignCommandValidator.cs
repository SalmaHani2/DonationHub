using FluentValidation;

namespace DonationHub.Application.Campaigns.Commands.UpdateCampaign;

public class UpdateCampaignCommandValidator
    : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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

        RuleFor(x => x.Campaign.RowVersion)
            .NotEmpty()
            .Must(value =>
            {
                try
                {
                    return Convert.FromBase64String(value).Length > 0;
                }
                catch
                {
                    return false;
                }
            })
            .WithMessage("A valid RowVersion is required.");
    }
}