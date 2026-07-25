namespace SocialNetwork.Identity.Application.Auth;

public interface IAuthSessionService
{
    Task SignInasync(UserAccount user, CancellationToken cancellationToken);
    Task SignOut();
}