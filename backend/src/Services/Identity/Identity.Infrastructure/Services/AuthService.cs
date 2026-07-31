using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using SocialNetwork.Identity.Application.Auth;
using SocialNetwork.Identity.Application.Email;
using SocialNetwork.Identity.Application.Users;

namespace SocialNetwork.Identity.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IAuthSessionService _authSession;
    private readonly IEmailSender _eSender;
    private readonly IConfiguration _config;

    public AuthService(IUserRepository users, IAuthSessionService authSession, IEmailSender eSender, IConfiguration config)
    {
        _users = users;
        _authSession = authSession;
        _eSender = eSender;
        _config = config;
    }
    public Task<AppResult> ConfirmEmailAsync(ConfirmEmailCommand command, CancellationToken cancellationToken) =>
        _users.ConfirmEmailAsync(command.UserId, command.Code, cancellationToken);

    public async Task ForgotPasswordAsync(string email, CancellationToken cancellationToken)
    {
        if (!await _users.IsEmailConfirmedAsync(email, cancellationToken))
            return;

        var user = await _users.FindByEmailAsync(email, cancellationToken);
        var code = await _users.GeneratePasswordResetTokenAsync(email, cancellationToken);

        if (user is null || code is null)
            return;

        var link = BuildFrontendLink("reset-password", new Dictionary<string, string?>
        {
            ["userId"] = user.Id,
            ["code"] = code
        });

        await _eSender.SendAsync(
            new EmailMessage(
                user.Email,
                "Reset your Social Network password",
                $"<p>Reset password: <a href=\"{link}\">Reset password</a></p>"),
            cancellationToken);
    }

    public async Task<SessionInfo?> GetSessionAsync(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var email = principal.FindFirstValue(ClaimTypes.Email);

        return userId is null || email is null 
            ? null : new SessionInfo(userId, email);
    }

    public async Task<AppResult> LoginAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        var user = await _users.VerifyPasswordAsync(command.Email, command.Password, cancellationToken);

        if(user is null)
            return AppResult.Failure(new AppError("invalid_credentials", "Invalid Credentials"));
        
        await _authSession.SignInAsync(user, cancellationToken);
        await _users.MarkAsLoginAsync(user.Id, cancellationToken);

        return AppResult.Success();
    }

    public Task LogoutAsync() => _authSession.SignOutAsync();

    public async Task<AppResult> RegisterAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var create = await _users.CreateAsync(
            command.Email,
            command.Password,
            cancellationToken
        );

        if(!create.Succeeded || create.Value is null) 
            return AppResult.Failure(create.Errors.ToArray());

        var code = await _users.GenerateEmailConfirmationTokenAsync(create.Value.Id, cancellationToken);

        if(code is null) 
            return AppResult.Failure(new AppError("token", "Confirmation token was not created"));


        var link = BuildFrontendLink("confirm-email", new Dictionary<string, string?>
        {
            ["userId"] = create.Value.Id,
            ["code"] = code
        });

        await _eSender.SendAsync(
            new EmailMessage(
                create.Value.Email,
                "Confrim your email",
                $"<p>Confirm your email: <a href=\"{link}\">Confirm account</a></p>"
            ),
            cancellationToken
        );

        return AppResult.Success();
    }

    public async Task<AppResult> ResetPasswordAsync(ResetPasswordCommand command, CancellationToken cancellationToken) => 
        await _users.ResetPasswordAsync(command.UserId, command.Code, command.NewPassword, cancellationToken);

    private string BuildFrontendLink(string path, IDictionary<string, string?> query)
    {
        var baseUrl = _config["Frontend:BaseUrl"]
            ?? throw new InvalidOperationException("Frontend:BaseUrl is missing");        
        
        return QueryHelpers.AddQueryString($"{baseUrl.TrimEnd('/')}/{path}", query);
    }
}