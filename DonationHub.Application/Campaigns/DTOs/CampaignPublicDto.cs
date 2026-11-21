namespace DonationHub.Application.Campaigns.DTOs;

public class CampaignPublicDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal GoalAmount { get; set; }
    public decimal RaisedAmount { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal ProgressPercent { get; set; }

    public int DaysLeft { get; set; }
}