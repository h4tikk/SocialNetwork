using System.ComponentModel.DataAnnotations;

namespace SocialNetwork.Identity.Api.Requests;

public sealed class ConfimEmailRequest
{
    [Required]
    public string UserId { get; init; } = string.Empty;
    [Required]
    public string Code { get; init; } = string.Empty;
}

public sealed class ForgotPasswordRequest
{
    [Required, EmailAddress]
    public string Email { get; init; } = string.Empty;
}

public sealed class ResetPasswordRequest
{
    [Required]
    public string UserId { get; init; } = string.Empty;
    [Required]
    public string Code { get; init; } = string.Empty;
    [Required, MinLength(10)]
    public string NewPassword { get; init; } = string.Empty;
}