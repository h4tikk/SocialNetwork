using System.Security.Claims;

namespace SocialNetwork.Identity.Application.Auth;

public interface IOidcAuthorizationService
{
    Task<ClaimsPrincipal?> CreatePrincipalAsync(
        ClaimsPrincipal identityCookiePrincipal,
        IEnumerable<string> requestedScopes,
        CancellationToken cancellationToken);
}