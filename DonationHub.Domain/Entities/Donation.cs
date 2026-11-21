
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class Donation : Entity
{
    public Guid CampaignId { get; private set; }
    public Guid DonorUserId { get; private set; }

    public Money Amount { get; private set; } = null!;

    public DonationStatus Status { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    public string? FailureReason { get; private set; }
    public string? ReceiptNumber { get; private set; }
    public string? ReceiptFileName { get; private set; }

    public Campaign Campaign { get; private set; } = null!;

    private Donation()
    {
    }

    public static Result<Donation> Create(
        Money amount,
        decimal minAmount,
        decimal maxAmount,
        DateTime currentTime)
    {
        if (amount is null)
        {
            return Result<Donation>.Failure(
                new Error(
                    "Donation.AmountRequired",
                    "Donation amount is required.",
                    ErrorType.Validation));
        }

        if (amount.Amount < minAmount || amount.Amount > maxAmount)
        {
            return Result<Donation>.Failure(
                new Error(
                    "Donation.AmountOutOfRange",
                    "Donation amount must be within the allowed range.",
                    ErrorType.Validation));
        }

        var donation = new Donation
        {
            Amount = amount,
            Status = DonationStatus.Pending,
            CreatedAt = currentTime
        };

        return Result<Donation>.Success(donation);
    }

    public Result MarkAsSucceeded(DateTime currentTime)
    {
        if (IsFinalState())
        {
            return Result.Failure(
                new Error(
                    "Donation.FinalState",
                    "A final donation state cannot be changed.",
                    ErrorType.Conflict));
        }

        Status = DonationStatus.Succeeded;
        CompletedAt = currentTime;

        return Result.Success();
    }

    public Result MarkAsFailed(
        string failureReason,
        DateTime currentTime)
    {
        if (IsFinalState())
        {
            return Result.Failure(
                new Error(
                    "Donation.FinalState",
                    "A final donation state cannot be changed.",
                    ErrorType.Conflict));
        }

        if (string.IsNullOrWhiteSpace(failureReason))
        {
            return Result.Failure(
                new Error(
                    "Donation.FailureReasonRequired",
                    "Failure reason is required.",
                    ErrorType.Validation));
        }

        Status = DonationStatus.Failed;
        FailureReason = failureReason;
        CompletedAt = currentTime;

        return Result.Success();
    }

    private bool IsFinalState()
    {
        return Status == DonationStatus.Succeeded ||
               Status == DonationStatus.Failed;
    }
}