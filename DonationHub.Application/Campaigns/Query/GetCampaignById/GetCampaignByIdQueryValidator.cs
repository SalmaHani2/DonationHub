
using FluentValidation;

namespace DonationHub.Application.Campaigns.Queries.GetCampaignById;

public class GetCampaignByIdQueryValidator
    : AbstractValidator<GetCampaignByIdQuery>
{
    public GetCampaignByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Campaign ID is required.");
    }
}
