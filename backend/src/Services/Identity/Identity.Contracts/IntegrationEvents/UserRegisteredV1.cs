namespace SocialNetwork.Identity.Contracts.IntegrationEvents;

public record UserRegisteredV1(
    string UserId,
    DateTimeOffset OccurredAtUtc
);