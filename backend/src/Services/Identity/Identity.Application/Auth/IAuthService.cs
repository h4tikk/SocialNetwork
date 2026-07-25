namespace SocialNetwork.Identity.Application.Auth;

public interface IAuthService
{
    Task<AppResult> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken);
    Task<AppResult> ConfirmEmailAsync(ConfirmEmailCommand command, CancellationToken cancellationToken);
    Task<AppResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken);
    Task LogoutAsync();
    SessionInfo? GetSession(System.Security.Claims.ClaimsPrincipal principal);
    Task ForgotPasswordAsync(string email, CancellationToken cancellationToken);
    Task<AppResult> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken);
}