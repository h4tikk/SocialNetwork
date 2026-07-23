using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;

namespace SocialNetwork.Identity.Infrastructure.OpenIddict;

public static class CertificateLoader
{
    public static X509Certificate2 LoadRequired(
        IConfiguration configuration,
        string pathKey,
        string passwordKey)
    {
        var path = configuration[pathKey]
            ?? throw new InvalidOperationException($"Missing configuration: {pathKey}");

        var password = configuration[passwordKey]
            ?? throw new InvalidOperationException($"Missing configuration: {passwordKey}");

        return new X509Certificate2(
            path,
            password,
            X509KeyStorageFlags.EphemeralKeySet);
    }
}