
using DonationHub.Application.Common.Results;

namespace DonationHub.Application.Identity;

public interface IIdentityService
{
    Task<IdentityUserResponse?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task<IdentityUserResponse?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken);

    Task<bool> CheckPasswordAsync(
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<Result<IdentityUserResponse>> CreateUserAsync(
        string email,
        string fullName,
        string preferredLanguage,
        string password,
        CancellationToken cancellationToken);

    Task<Result> AddToRoleAsync(
        string email,
        string role,
        CancellationToken cancellationToken);

    Task<Result> DeleteUserAsync(
        string email,
        CancellationToken cancellationToken);
}
