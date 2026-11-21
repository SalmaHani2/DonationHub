
using DonationHub.Domain.Common;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace DonationHub.UnitTests.Domain;

public class CampaignTests
{
    private readonly FakeTimeProvider _timeProvider;

    public CampaignTests()
    {
        _timeProvider = new FakeTimeProvider(
            new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero));
    }

    private DateTime CurrentTime =>
        _timeProvider.GetUtcNow().UtcDateTime;

    private Money CreateMoney(decimal amount)
    {
        return Money.Create(amount, "USD");
    }

    private Campaign CreateCampaign(decimal goalAmount = 100)
    {
        var result = Campaign.Create(
            CreateMoney(goalAmount),
            CurrentTime.AddDays(10),
            CurrentTime);

        return result.Value!;
    }

    //  Goal amount must be greater than zero
    [Fact]
    public void Create_WhenGoalAmountIsZero_ShouldReturnFailure()
    {
        // Arrange
        var goal = CreateMoney(0);

        // Act
        var result = Campaign.Create(
            goal,
            CurrentTime.AddDays(10),
            CurrentTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Campaign.InvalidGoalAmount");
    }

    //  End date cannot be in the past

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    public void Create_WhenEndDateIsNotInFuture_ShouldReturnFailure(
        int days)
    {
        // Arrange
        var endDate = CurrentTime.AddDays(days);

        // Act
        var result = Campaign.Create(
            CreateMoney(100),
            endDate,
            CurrentTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Campaign.EndDateInPast");
    }

    // Goal cannot be updated below raised amount

    [Fact]
    public void ChangeGoalAmount_WhenGoalIsBelowRaisedAmount_ShouldReturnFailure()
    {
        // Arrange
        var campaign = CreateCampaign(100);

        campaign.AddSucceededDonation(CreateMoney(60));

        // Act
        var result = campaign.ChangeGoalAmount(CreateMoney(50));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Campaign.GoalBelowRaised");
    }

    // A campaign in a final state cannot be updated

    [Fact]
    public void Update_WhenCampaignIsCompleted_ShouldReturnFailure()
    {
        // Arrange
        var campaign = CreateCampaign(100);

        campaign.AddSucceededDonation(CreateMoney(100));

        // Act
        var result = campaign.Update(
            "حملة جديدة",
            "New Campaign",
            "وصف جديد",
            "New Description",
            CreateMoney(200),
            CurrentTime.AddDays(20),
            Guid.NewGuid(),
            CurrentTime);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Campaign.NotActive");
    }

    // Campaign automatically transitions to Completed when goal is reached

    [Fact]
    public void AddSucceededDonation_WhenGoalIsReached_ShouldCompleteCampaign()
    {
        // Arrange
        var campaign = CreateCampaign(100);

        // Act
        var result = campaign.AddSucceededDonation(CreateMoney(100));

        // Assert
        result.IsSuccess.Should().BeTrue();
        campaign.Status.Should().Be(CampaignStatus.Completed);
    }
}
