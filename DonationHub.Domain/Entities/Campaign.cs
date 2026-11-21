
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class Campaign : Entity, ISoftDeletable
{
    public string TitleAr { get; private set; } = string.Empty;
    public string TitleEn { get; private set; } = string.Empty;
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;

    public Money GoalAmount { get; private set; } = null!;
    public Money RaisedAmount { get; private set; } = null!;

    public DateTime EndDate { get; private set; }

    public Guid CategoryId { get; private set; }
    public string? BannerFileName { get; private set; }

    public CampaignStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public Category Category { get; private set; } = null!;
    public ICollection<Donation> Donations { get; private set; } = new List<Donation>();

    public byte[] RowVersion { get; private set; } = null!;

    private Campaign()
    {
    }

    public static Result<Campaign> Create(
        Money goalAmount,
        DateTime endDate,
        DateTime currentTime)
    {

        //if (goalAmount.Amount <= 0)
        //    throw new ArgumentException("GoalAmount must be greater than zero.");
        if (goalAmount is null)
            return Result<Campaign>.Failure(
                new Error(
                    "Campaign.GoalAmountRequired",
                    "Goal amount is required.",
                    ErrorType.Validation));

        if (goalAmount.Amount <= 0)
            return Result<Campaign>.Failure(
                new Error(
                    "Campaign.InvalidGoalAmount",
                    "Goal amount must be greater than zero.",
                    ErrorType.Validation));

        if (endDate <= currentTime)
            return Result<Campaign>.Failure(
                new Error(
                    "Campaign.EndDateInPast",
                    "Campaign end date must be in the future.",
                    ErrorType.Validation));

        var campaign = new Campaign
        {
            GoalAmount = goalAmount,
            RaisedAmount = Money.Create(0m, goalAmount.Currency),
            EndDate = endDate,
            Status = CampaignStatus.Active
        };

        return Result<Campaign>.Success(campaign);
    }

    public Result EnsureCanAcceptDonation(DateTime currentTime)
    {
        if (Status != CampaignStatus.Active)
            return Result.Failure(
                new Error(
                    "Campaign.NotActive",
                    "Campaign is not active.",
                    ErrorType.Conflict));

        if (EndDate <= currentTime)
            return Result.Failure(
                new Error(
                    "Campaign.Expired",
                    "Campaign has expired.",
                    ErrorType.Conflict));

        return Result.Success();
    }

    public Result ChangeGoalAmount(Money goalAmount)
    {
        if (goalAmount is null)
            return Result.Failure(
                new Error(
                    "Campaign.GoalAmountRequired",
                    "Goal amount is required.",
                    ErrorType.Validation));

        if (goalAmount.Amount <= 0)
            return Result.Failure(
                new Error(
                    "Campaign.InvalidGoalAmount",
                    "Goal amount must be greater than zero.",
                    ErrorType.Validation));

        if (goalAmount.Currency != RaisedAmount.Currency)
            return Result.Failure(
                new Error(
                    "Campaign.CurrencyMismatch",
                    "Currency mismatch.",
                    ErrorType.Validation));

        if (goalAmount.Amount < RaisedAmount.Amount)
            return Result.Failure(
                new Error(
                    "Campaign.GoalBelowRaised",
                    "Goal amount cannot be lower than raised amount.",
                    ErrorType.Validation));

        GoalAmount = goalAmount;

        return Result.Success();
    }

    public Result ChangeEndDate(
        DateTime endDate,
        DateTime currentTime)
    {
        if (endDate <= currentTime)
            return Result.Failure(
                new Error(
                    "Campaign.EndDateInPast",
                    "Campaign end date must be in the future.",
                    ErrorType.Validation));

        EndDate = endDate;

        return Result.Success();
    }

    public Result AddSucceededDonation(Money amount)
    {
        if (amount is null)
            return Result.Failure(
                new Error(
                    "Campaign.DonationAmountRequired",
                    "Donation amount is required.",
                    ErrorType.Validation));

        if (IsFinalState())
            return Result.Failure(
                new Error(
                    "Campaign.FinalState",
                    "A final campaign state cannot be changed.",
                    ErrorType.Conflict));

        if (amount.Amount <= 0)
            return Result.Failure(
                new Error(
                    "Campaign.InvalidDonationAmount",
                    "Donation amount must be greater than zero.",
                    ErrorType.Validation));

        if (amount.Currency != RaisedAmount.Currency)
            return Result.Failure(
                new Error(
                    "Campaign.CurrencyMismatch",
                    "Currency mismatch.",
                    ErrorType.Validation));

        var newRaisedAmount = Money.Create(
            RaisedAmount.Amount + amount.Amount,
            RaisedAmount.Currency);

        RaisedAmount = newRaisedAmount;

        if (RaisedAmount.Amount >= GoalAmount.Amount)
            Status = CampaignStatus.Completed;

        return Result.Success();
    }

    public Result Deleted(DateTime deletedAt)
    {
        IsDeleted = true;
        DeletedAtUtc = deletedAt;

        return Result.Success();
    }

    public Result UndoDelete()
    {
        IsDeleted = false;
        DeletedAtUtc = null;

        return Result.Success();
    }

    public Result Update(
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        Money goalAmount,
        DateTime endDate,
        Guid categoryId,
        DateTime currentTime)
    {
        if (Status != CampaignStatus.Active)
            return Result.Failure(
                new Error(
                    "Campaign.NotActive",
                    "Only active campaigns can be updated.",
                    ErrorType.Conflict));

        if (goalAmount is null)
            return Result.Failure(
                new Error(
                    "Campaign.GoalAmountRequired",
                    "Goal amount is required.",
                    ErrorType.Validation));

        if (goalAmount.Amount <= 0)
            return Result.Failure(
                new Error(
                    "Campaign.InvalidGoalAmount",
                    "Goal amount must be greater than zero.",
                    ErrorType.Validation));

        if (endDate <= currentTime)
            return Result.Failure(
                new Error(
                    "Campaign.EndDateInPast",
                    "Campaign end date must be in the future.",
                    ErrorType.Validation));

        if (goalAmount.Amount < RaisedAmount.Amount)
            return Result.Failure(
                new Error(
                    "Campaign.GoalBelowRaised",
                    "Goal amount cannot be less than raised amount.",
                    ErrorType.Validation));

        if (goalAmount.Currency != RaisedAmount.Currency)
            return Result.Failure(
                new Error(
                    "Campaign.CurrencyMismatch",
                    "Currency mismatch.",
                    ErrorType.Validation));

        TitleAr = titleAr;
        TitleEn = titleEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;

        GoalAmount = goalAmount;
        EndDate = endDate;
        CategoryId = categoryId;
        UpdatedAt = currentTime;

        return Result.Success();
    }

    private bool IsFinalState()
    {
        return Status == CampaignStatus.Completed ||
               Status == CampaignStatus.Expired;
    }
}
