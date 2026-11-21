using DonationHub.Application.Campaigns.Commands.CreateCampaign;
using DonationHub.Application.Common;
using DonationHub.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Application;

public class CreateCampaignCommandHandlerTests
{
    private readonly Mock<IUnitOfWorks> _unitOfWork = new();
    private readonly Mock<IGenericRepository<Category>> _categoryRepository = new();
    private readonly Mock<IGenericRepository<Campaign>> _campaignRepository = new();
    private static void SetRowVersion(Campaign campaign)
    {
        typeof(Campaign)
            .GetProperty("RowVersion")!
            .SetValue(campaign, new byte[] { 1, 2, 3 });
    }

    private readonly FakeTimeProvider _timeProvider = new(
        new DateTimeOffset(2026, 11, 21, 12, 0, 0, TimeSpan.Zero));

    public CreateCampaignCommandHandlerTests()
    {
        _unitOfWork
            .Setup(x => x.Repository<Category>())
            .Returns(_categoryRepository.Object);

        _unitOfWork
            .Setup(x => x.Repository<Campaign>())
            .Returns(_campaignRepository.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ShouldReturnNotFoundWithoutSaving()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        _categoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        var handler = new CreateCampaignCommandHandler(
            _unitOfWork.Object,
            _timeProvider);

        var command = CreateCommand(categoryId);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Category.NotFound");

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldAddCampaignAndSaveOnce()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = CreateCategory();

        _categoryRepository
            .Setup(x => x.GetByIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _campaignRepository
            .Setup(x => x.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _unitOfWork
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var handler = new CreateCampaignCommandHandler(
            _unitOfWork.Object,
            _timeProvider);

        // Act
        var result = await handler.Handle(
            CreateCommand(categoryId),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _campaignRepository.Verify(
            x => x.AddAsync(It.IsAny<Campaign>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWork.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private CreateCampaignCommand CreateCommand(Guid categoryId)
    {
        return new CreateCampaignCommand(
            new DonationHub.Application.Campaigns.DTOs.CreateCampaignDto
            {
                TitleAr = "حملة",
                TitleEn = "Campaign",
                DescriptionAr = "وصف",
                DescriptionEn = "Description",
                GoalAmount = 100,
                EndDate = _timeProvider.GetUtcNow().UtcDateTime.AddDays(10),
                CategoryId = categoryId
            },
            Guid.NewGuid());
    }

    private Category CreateCategory()
    {
        return Category.Create(
            "Category Ar",
            "Category En",
            "Description Ar",
            "Description En",
            _timeProvider.GetUtcNow().UtcDateTime);
    }
}