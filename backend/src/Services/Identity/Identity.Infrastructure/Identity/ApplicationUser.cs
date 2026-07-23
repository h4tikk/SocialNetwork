using Microsoft.AspNetCore.Identity;

namespace SocialNetwork.Identity.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastLoginAtUtc { get; set; }
}