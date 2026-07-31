namespace SocialNetwork.Identity.Application.Auth;

public interface IAuthSessionService
{
    Task SignInAsync(UserAccount user, CancellationToken cancellationToken);
    Task SignOutAsync();
}