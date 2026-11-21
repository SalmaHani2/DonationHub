using DonationHub.Application.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Campaigns.DTOs;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace DonationHub.UnitTests.Application;

public class CreateCampaignCommandValidatorTests
{
    private readonly FakeTimeProvider _timeProvider;
    private readonly CreateCampaignCommandValidator _validator;

    public CreateCampaignCommandValidatorTests()
    {
        _timeProvider = new FakeTimeProvider(
            new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero));

        _validator = new CreateCampaignCommandValidator(_timeProvider);
    }

    [Fact]
    public async Task Validate_WhenTitlesAreMissing_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Campaign.TitleAr = "";
        command.Campaign.TitleEn = "";

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Campaign.TitleAr");
        result.Errors.Should().Contain(x => x.PropertyName == "Campaign.TitleEn");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Validate_WhenGoalAmountIsNotPositive_ShouldFail(decimal amount)
    {
        // Arrange
        var command = CreateValidCommand();
        command.Campaign.GoalAmount = amount;

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Campaign.GoalAmount");
    }

    [Fact]
    public async Task Validate_WhenCategoryIdIsEmpty_ShouldFail()
    {
        // Arrange
        var command = CreateValidCommand();
        command.Campaign.CategoryId = Guid.Empty;

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Campaign.CategoryId");
    }

    private CreateCampaignCommand CreateValidCommand()
    {
        return new CreateCampaignCommand(
            new CreateCampaignDto
            {
                TitleAr = "حملة",
                TitleEn = "Campaign",
                DescriptionAr = "وصف",
                DescriptionEn = "Description",
                GoalAmount = 100,
                EndDate = _timeProvider.GetUtcNow().UtcDateTime.AddDays(10),
                CategoryId = Guid.NewGuid()
            },
            Guid.NewGuid());
    }
}