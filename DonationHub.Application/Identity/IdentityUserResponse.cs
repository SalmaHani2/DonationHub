namespace DonationHub.Application.Identity;

public class IdentityUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PreferredLanguage { get; set; } = "en";
    public bool IsActive { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}