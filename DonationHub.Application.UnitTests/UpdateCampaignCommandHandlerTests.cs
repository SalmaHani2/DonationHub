
using DonationHub.Application.Campaigns.Commands.UpdateCampaign;
using DonationHub.Application.Campaigns.DTOs;
using DonationHub.Application.Common;
using DonationHub.Domain.Common;
using DonationHub.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;
using System.Runtime.CompilerServices;

namespace DonationHub.UnitTests.Application;

public class UpdateCampaignCommandHandlerTests
{
    private readonly Mock<IUnitOfWorks> _unitOfWork = new();
    private readonly Mock<IGenericRepository<Campaign>> _campaignRepository = new();
    private readonly Mock<IGenericRepository<Category>> _categoryRepository = new();

    private readonly FakeTimeProvider _timeProvider = new(
        new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero));

    public UpdateCampaignCommandHandlerTests()
    {
        _unitOfWork
            .Setup(x => x.Repository<Campaign>())
            .Returns(_campaignRepository.Object);

        _unitOfWork
            .Setup(x => x.Repository<Category>())
            .Returns(_categoryRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenCampaignDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var campaignId = Guid.NewGuid();

        _campaignRepository
            .Setup(x => x.GetByIdAsync(
                campaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Campaign?)null);

        var handler = new UpdateCampaignCommandHandler(
            _unitOfWork.Object,
            _timeProvider);

        // Act
        var result = await handler.Handle(
            CreateCommand(campaignId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Campaign.NotFound");

        _unitOfWork.Verify(
            x => x.TrySaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenConcurrencyConflictOccurs_ShouldReturnConcurrencyConflict()
    {
        // Arrange
        var campaignId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        var now = _timeProvider.GetUtcNow().UtcDateTime;

        var campaignResult = Campaign.Create(
            Money.Create(100, "USD"),
            now.AddDays(10),
            now);

        var campaign = campaignResult.Value!;

        var category = (Category)RuntimeHelpers.GetUninitializedObject(
            typeof(Category));

        _campaignRepository
            .Setup(x => x.GetByIdAsync(
                campaignId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(campaign);

        _categoryRepository
            .Setup(x => x.GetByIdAsync(
                categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _campaignRepository
            .Setup(x => x.UpdateAsync(
                It.IsAny<Campaign>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(x => x.TrySaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new UpdateCampaignCommandHandler(
            _unitOfWork.Object,
            _timeProvider);

        // Act
        var result = await handler.Handle(
            CreateCommand(campaignId, categoryId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Campaign.ConcurrencyConflict");

        _campaignRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.TrySaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private UpdateCampaignCommand CreateCommand(
        Guid campaignId,
        Guid? categoryId = null)
    {
        return new UpdateCampaignCommand(
            campaignId,
            new UpdateCampaignDto
            {
                TitleAr = "حملة جديدة",
                TitleEn = "New Campaign",
                DescriptionAr = "وصف جديد",
                DescriptionEn = "New Description",
                GoalAmount = 200,
                EndDate = _timeProvider.GetUtcNow().UtcDateTime.AddDays(20),
                CategoryId = categoryId ?? Guid.NewGuid(),
                RowVersion = Convert.ToBase64String(new byte[] { 1, 2, 3 })
            });
    }
}