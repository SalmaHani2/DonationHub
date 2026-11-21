using DonationHub.Application.Auth.Commands.Register;
using DonationHub.Application.Auth.Auth;
using FluentAssertions;
using Xunit;

namespace DonationHub.UnitTests.Application;

public class RegisterAsyncCommandValidatorTests
{
    private readonly RegisterAsyncCommandValidator _validator = new();

    [Theory]
    [InlineData("short1A")]
    [InlineData("password1")]
    [InlineData("PASSWORD1")]
    [InlineData("Password")]
    [InlineData("password")]
    public async Task Validate_WhenPasswordBreaksPolicy_ShouldFail(string password)
    {
        // Arrange
        var command = CreateCommand(password);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        result.IsValid.Should().BeFalse();
    }

    private static RegisterAsyncCommand CreateCommand(string password)
    {
        return new RegisterAsyncCommand
        {
            Request = new RegisterRequest
            {
                Email = "test@example.com",
                FullName = "Test User",
                Password = password
            }
        };
    }
}