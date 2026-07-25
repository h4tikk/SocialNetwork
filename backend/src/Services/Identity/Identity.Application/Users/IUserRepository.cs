using SocialNetwork.Identity.Application.Auth;

namespace SocialNetwork.Identity.Application.Users;

public interface IUserRepository
{
    Task<UserAccount?> FindByIdAsync(string userId, CancellationToken cancellationToken);
    Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    Task<AppResult<UserAccount>> CreateAsync(string email, string password, CancellationToken cancellationToken);
    Task<UserAccount?> VerifyPasswordAsync(string email, string password, CancellationToken cancellationToken);
    Task<AppResult> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken);
    Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken);
    Task<string?> GeneratePasswordResetTokenAsync(string password, CancellationToken cancellationToken);
    Task<bool> IsEmailConfirmedAsync(string email, CancellationToken cancellationToken);
    Task<AppResult> ResetPasswordAsync(string userId, string code, string newPassword, CancellationToken cancellationToken);
    Task MarkAsLoginAsync(string userId, CancellationToken cancellationToken);

}