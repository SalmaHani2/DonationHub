
using DonationHub.Application.Admin.Options;
using DonationHub.Application.Common.Errors;
using DonationHub.Application.Common.Results;
using DonationHub.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace DonationHub.Infrastructure.Seed;

public class AdminSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly SeedAdminOptions _options;
    private readonly TimeProvider _timeProvider;

    private static readonly string[] Roles =
    {
        "Admin",
        "CampaignManager",
        "Donor"
    };

    public AdminSeeder(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<SeedAdminOptions> options,
        TimeProvider timeProvider)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _options = options.Value;
        _timeProvider = timeProvider;
    }

    public async Task<Result> SeedAsync(IServiceProvider services)
    {
        // Create roles if they do not exist.
        foreach (var role in Roles)
        {
            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                var result = await _roleManager.CreateAsync(
                    new IdentityRole<Guid>(role));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(error => error.Code));

                    return Result.Failure(
                        new Error(
                            "Seed.RoleCreationFailed",
                            $"Could not create role '{role}': {errors}",
                            ErrorType.Failure));
                }
            }
        }

        // Find the initial Admin.
        var admin = await _userManager.FindByEmailAsync(
            _options.Email);

        if (admin == null)
        {
            admin = new ApplicationUser(
                _options.Email,
                _options.FullName,
                "en",
                _timeProvider.GetUtcNow().UtcDateTime);

            var createResult = await _userManager.CreateAsync(
                admin,
                _options.Password);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    createResult.Errors.Select(error => error.Code));

                return Result.Failure(
                    new Error(
                        "Seed.AdminCreationFailed",
                        $"Could not create the initial Admin: {errors}",
                        ErrorType.Failure));
            }
        }

        // Ensure the user has the Admin role.
        var isAdmin = await _userManager.IsInRoleAsync(
            admin,
            "Admin");

        if (!isAdmin)
        {
            var roleResult = await _userManager.AddToRoleAsync(
                admin,
                "Admin");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(error => error.Code));

                return Result.Failure(
                    new Error(
                        "Seed.AdminRoleAssignmentFailed",
                        $"Could not assign the Admin role: {errors}",
                        ErrorType.Failure));
            }
        }

        return Result.Success();
    }
}
