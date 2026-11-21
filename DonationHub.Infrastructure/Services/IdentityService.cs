
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Application.Identity;
using Microsoft.AspNetCore.Identity;

namespace DonationHub.Infrastructure.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TimeProvider _timeProvider;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        TimeProvider timeProvider)
    {
        _userManager = userManager;
        _timeProvider = timeProvider;
    }

    // Find User By Email
    public async Task<IdentityUserResponse?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return MapToResponse(user, roles);
    }

    // Find User By Id
    public async Task<IdentityUserResponse?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return MapToResponse(user, roles);
    }

    // Check Password
    public async Task<bool> CheckPasswordAsync(
        string email,
        string password,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
            return false;

        return await _userManager.CheckPasswordAsync(user, password);
    }

    // Create User
    public async Task<Result<IdentityUserResponse>> CreateUserAsync(
        string email,
        string fullName,
        string preferredLanguage,
        string password,
        CancellationToken cancellationToken)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser != null)
        {
            return Result<IdentityUserResponse>.Failure(
                ErrorCatalog.Auth.EmailAlreadyExists);
        }

        var user = new ApplicationUser(
            email,
            fullName,
            preferredLanguage,
            _timeProvider.GetUtcNow().UtcDateTime);

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Code));

            return Result<IdentityUserResponse>.Failure(
                new Error(
                    "Auth.UserCreationFailed",
                    errors,
                    ErrorType.Failure));
        }

        return Result<IdentityUserResponse>.Success(
            MapToResponse(user, new List<string>()));
    }

    // Add User To Role
    public async Task<Result> AddToRoleAsync(
        string email,
        string role,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Result.Failure(
                new Error(
                    "Admin.UserNotFound",
                    "User was not found.",
                    ErrorType.NotFound));
        }

        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Code));

            return Result.Failure(
                new Error(
                    "Admin.AddToRoleFailed",
                    errors,
                    ErrorType.Failure));
        }

        return Result.Success();
    }

    // Delete User
    public async Task<Result> DeleteUserAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Result.Failure(
                new Error(
                    "Admin.UserNotFound",
                    "User was not found.",
                    ErrorType.NotFound));
        }

        var result = await _userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(error => error.Code));

            return Result.Failure(
                new Error(
                    "Admin.UserDeletionFailed",
                    errors,
                    ErrorType.Failure));
        }

        return Result.Success();
    }

    // Map User To Response
    private static IdentityUserResponse MapToResponse(
        ApplicationUser user,
        IList<string> roles)
    {
        return new IdentityUserResponse
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            PreferredLanguage = user.PreferredLanguage,
            IsActive = user.IsActive,
            Roles = roles
        };
    }
}
