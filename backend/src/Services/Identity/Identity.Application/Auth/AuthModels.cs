namespace SocialNetwork.Identity.Application.Auth;

public sealed record AppError(string Code, string Description);

public class AppResult
{
    public bool Succeeded { get; }
    public IReadOnlyList<AppError> Errors { get; }
    public AppResult(bool succeeded, IReadOnlyList<AppError> errors)
    {
        Succeeded = succeeded;
        Errors = errors;
    }

    public static AppResult Success() => new(true, []);
    public static AppResult Failure(params AppError[] errors) => new(false, errors);
}

public sealed class AppResult<T> : AppResult
{
    public T? Value { get; }

    private AppResult(bool succeeded, T? value, IReadOnlyList<AppError> errors)
        :base(succeeded, errors)
    {
        Value = value;
    }

    public static AppResult<T> Success(T value) => new(true, value, []);
    public new static AppResult<T> Failure(params AppError[] errors) => new(false, default, errors);
}

public sealed record UserAccount(
    string Id,
    string Email,
    bool EmailConfrimed,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? LastLoginAtUtc,
    string SecurityStamp
);

public sealed record RegisterUserCommand(string Email, string Password);
public sealed record LoginCommand(string Email, string Password);
public sealed record ConfirmEmailCommand(string UserId, string Code);
public sealed record ResetPasswordCommand(string UserId, string Code, string NewPassword);
public sealed record SessionInfo(string UserId, string Email);