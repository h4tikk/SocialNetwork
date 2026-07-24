using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Identity.Api.Requests;

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;
    [Required, MinLength(8)]
    public string Password { get; init; } = string.Empty;
}

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;
    [Required]
    public string Password { get; init; } = string.Empty;
}