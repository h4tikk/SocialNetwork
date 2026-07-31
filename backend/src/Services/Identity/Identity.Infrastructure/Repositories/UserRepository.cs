using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Identity;
using SocialNetwork.Identity.Application.Auth;
using SocialNetwork.Identity.Application.Users;
using SocialNetwork.Identity.Infrastructure.Identity;

namespace SocialNetwork.Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    public UserRepository(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }
    
    public async Task<AppResult> ConfirmEmailAsync(string userId, string code, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if(user is null) return AppResult.Failure(new AppError("not_found", "User was not found"));

        return ToAppResult(await _userManager.ConfirmEmailAsync(user, code));
    }

    public async Task<AppResult<UserAccount>> CreateAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);
        return result.Succeeded 
            ? AppResult<UserAccount>.Success(Map(user))
            : Failure<UserAccount>(result);
    }

    public async Task<UserAccount?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);
        return user is null ? null : Map(user);
    }

    public async Task<UserAccount?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return user is null ? null : Map(user);

    }

    public async Task<string?> GenerateEmailConfirmationTokenAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        return user is null ? null : await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user is null ? null : await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    public async Task<bool> IsEmailConfirmedAsync(string email, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user is not null && await _userManager.IsEmailConfirmedAsync(user);
    }

    public async Task MarkAsLoginAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if(user is null) return;

        user.LastLoginAtUtc = DateTimeOffset.Now;
        await _userManager.UpdateAsync(user);
    }

    public async Task<AppResult> ResetPasswordAsync(string userId, string code, string newPassword, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if(user is null) return AppResult.Failure(new AppError("not_found", "User was not found"));

        return ToAppResult(await _userManager.ResetPasswordAsync(user, code, newPassword));
    }

    public async Task<UserAccount?> VerifyPasswordAsync(string email, string password, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if(user is null) return null;

        if (!await _userManager.IsEmailConfirmedAsync(user)
            || await _userManager.IsLockedOutAsync(user))
            return null;
            var isValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isValid)
        {
            if (_userManager.SupportsUserLockout)
                await _userManager.AccessFailedAsync(user);

            return null;
        }

        if (_userManager.SupportsUserLockout)
            await _userManager.ResetAccessFailedCountAsync(user);

        return Map(user);
    }

    private static UserAccount Map(ApplicationUser user) => new(
        user.Id,
        user.Email ?? string.Empty,
        user.EmailConfirmed,
        user.CreatedAtUtc,
        user.LastLoginAtUtc,
        user.SecurityStamp ?? string.Empty
    );

    private static AppResult ToAppResult(IdentityResult result) =>
        result.Succeeded
            ? AppResult.Success()
            : AppResult.Failure(result.Errors
                .Select(error => new AppError(error.Code, error.Description))
                .ToArray());
    
    private static AppResult<T> Failure<T>(IdentityResult result) =>
        AppResult<T>.Failure(result.Errors
            .Select(errors => new AppError(errors.Code, errors.Description))
            .ToArray());
}