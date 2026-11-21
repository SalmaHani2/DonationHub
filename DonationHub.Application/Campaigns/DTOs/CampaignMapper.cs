using DonationHub.Domain.Entities;

namespace DonationHub.Application.Campaigns.DTOs;

public static class CampaignMapper
{
    public static CampaignAdminDto ToAdminDto(Campaign campaign)
    {
        return new CampaignAdminDto
        {
            Id = campaign.Id,

            TitleAr = campaign.TitleAr,
            TitleEn = campaign.TitleEn,

            DescriptionAr = campaign.DescriptionAr,
            DescriptionEn = campaign.DescriptionEn,

            GoalAmount = campaign.GoalAmount.Amount,
            RaisedAmount = campaign.RaisedAmount.Amount,
           

            EndDate = campaign.EndDate,

            CategoryId = campaign.CategoryId,

            Status = campaign.Status.ToString(),

            RowVersion = campaign.RowVersion == null
             ? string.Empty
             : Convert.ToBase64String(campaign.RowVersion)
        };
    }

    public static CampaignPublicDto ToPublicDto(
        Campaign campaign,
        string language,
        TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;

        var progress = campaign.GoalAmount.Amount <= 0
            ? 0
            : Math.Min(
                100,
                campaign.RaisedAmount.Amount /
                campaign.GoalAmount.Amount * 100);

        var daysLeft = Math.Max(
            0,
            (int)Math.Ceiling((campaign.EndDate - now).TotalDays));

        var useArabic = language.Equals(
            "ar",
            StringComparison.OrdinalIgnoreCase);

        return new CampaignPublicDto
        {
            Id = campaign.Id,

            Title = useArabic
                ? campaign.TitleAr
                : campaign.TitleEn,

            Description = useArabic
                ? campaign.DescriptionAr
                : campaign.DescriptionEn,

            GoalAmount = campaign.GoalAmount.Amount,
            RaisedAmount = campaign.RaisedAmount.Amount,

            EndDate = campaign.EndDate,

            Status = campaign.Status.ToString(),

            ProgressPercent = progress,
            DaysLeft = daysLeft
        };
    }
}