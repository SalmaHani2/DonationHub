
using DonationHub.Application.Auth.Auth;
using DonationHub.Application.Auth.Commands.Login;
using DonationHub.Application.Auth.Options;
using DonationHub.Application.Common;
using DonationHub.Application.Identity;
using DonationHub.Application.Tokens;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DonationHub.UnitTests.Application;

public class LoginAsyncHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IUnitOfWorks> _unitOfWork = new();
    private readonly Mock<ITokenService> _tokenService = new();

    private readonly LoginAsyncHandler _handler;

    public LoginAsyncHandlerTests()
    {
        _handler = new LoginAsyncHandler(
            _identityService.Object,
            _unitOfWork.Object,
            _tokenService.Object,
            TimeProvider.System,
            Options.Create(new JwtOptions()));
    }

    [Fact]
    public async Task Handle_WhenEmailDoesNotExist_ShouldReturnInvalidCredentials()
    {
        // Arrange
        var email = "unknown@example.com";

        _identityService
            .Setup(x => x.FindByEmailAsync(
                email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityUserResponse?)null);

        var command = CreateCommand(email, "Password123");

        // Act
        var result = await _handler.Handle(
            command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Auth.InvalidCredentials");

        _identityService.Verify(
            x => x.CheckPasswordAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenPasswordIsWrong_ShouldReturnInvalidCredentials()
    {
        // Arrange
        var email = "user@example.com";

        var user = new IdentityUserResponse
        {
            Id = Guid.NewGuid(),
            Email = email,
            FullName = "Test User",
            PreferredLanguage = "en",
            IsActive = true
        };

        _identityService
            .Setup(x => x.FindByEmailAsync(
                email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _identityService
            .Setup(x => x.CheckPasswordAsync(
                email,
                "WrongPassword123",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = CreateCommand(email, "WrongPassword123");

        // Act
        var result = await _handler.Handle(
            command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error!.Code.Should().Be("Auth.InvalidCredentials");
    }

    private static LoginAsyncCommand CreateCommand(
        string email,
        string password)
    {
        return new LoginAsyncCommand(
            new LoginRequest
            {
                Email = email,
                Password = password
            });
    }
}