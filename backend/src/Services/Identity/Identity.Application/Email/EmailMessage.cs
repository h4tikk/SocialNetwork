namespace SocialNetwork.Identity.Application.Email;

public record EmailMessage(
    string To,
    string Subject,
    string HtmlBody
);