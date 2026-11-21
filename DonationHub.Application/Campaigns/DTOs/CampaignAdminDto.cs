namespace DonationHub.Application.Campaigns.DTOs;

public class CampaignAdminDto
{
    public Guid Id { get; set; }

    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;

    public string DescriptionAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;

    public decimal GoalAmount { get; set; }
    public decimal RaisedAmount { get; set; }

    public DateTime EndDate { get; set; }

    public Guid CategoryId { get; set; }

    public string Status { get; set; } = string.Empty;

    public string RowVersion { get; set; } = string.Empty;
}