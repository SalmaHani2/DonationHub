
using DonationHub.Domain.Common;
using DonationHub.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace DonationHub.UnitTests.Domain;

public class CategoryTests
{
    // 8. Deleting a category that has campaigns returns Category.HasCampaigns

    [Fact]
    public void EnsureCanBeDeleted_WhenCategoryHasCampaigns_ShouldReturnFailure()
    {
        // Arrange
        var currentTime = new DateTime(2026, 11, 21);

        var category = Category.Create(
            "تبرعات",
            "Donations",
            null,
            null,
            currentTime);

        var campaignResult = Campaign.Create(
            Money.Create(100, "USD"),
            currentTime.AddDays(10),
            currentTime);

        var campaign = campaignResult.Value!;

        category.Campaigns.Add(campaign);

        // Act
        var result = category.EnsureCanBeDeleted();

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Category.HasCampaigns");
    }
}
