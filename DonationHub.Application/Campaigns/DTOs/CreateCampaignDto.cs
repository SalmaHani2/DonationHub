namespace DonationHub.Application.Campaigns.DTOs;

public class CreateCampaignDto
{
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;

    public string DescriptionAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;

    public decimal GoalAmount { get; set; }

    public DateTime EndDate { get; set; }

    public Guid CategoryId { get; set; }
}