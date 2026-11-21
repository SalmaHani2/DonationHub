
using DonationHub.Application.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common.Behaviors;
using DonationHub.Application.Common.Results;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Time.Testing;
using Xunit;

namespace DonationHub.UnitTests.Application;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WhenValidationFails_ShouldReturnFailureWithoutCallingHandler()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider(
            new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero));

        var validator = new CreateCampaignCommandValidator(timeProvider);

        var behavior =
            new ValidationBehavior<
                CreateCampaignCommand,
                Result<CampaignAdminDto>>(
                    new[] { validator });

        var command = new CreateCampaignCommand(
            new CreateCampaignDto
            {
                TitleAr = "",
                TitleEn = "",
                DescriptionAr = "Description",
                DescriptionEn = "Description",
                GoalAmount = 100,
                EndDate = timeProvider.GetUtcNow().UtcDateTime.AddDays(10),
                CategoryId = Guid.NewGuid()
            },
            Guid.NewGuid());

        var handlerCalled = false;

        RequestHandlerDelegate<Result<CampaignAdminDto>> next =
            (CancellationToken _) =>
            {
                handlerCalled = true;

                return Task.FromResult(
                    Result<CampaignAdminDto>.Success(null!));
            };

        // Act
        var result = await behavior.Handle(
            command,
            next,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Validation.Failed");
        handlerCalled.Should().BeFalse();
    }
}