using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SocialNetwork.Identity.Application.Email;

namespace SocialNetwork.Identity.Infrastructure.Email;

public class SmtpEmailSender : IEmailSender
{
    private readonly SmtpOptions _options;

    public SmtpEmailSender(IOptions<SmtpOptions> options)
    {
        _options = options.Value;
    }
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        using var mail = new MailMessage(_options.From, message.To)
        {
            Subject = message.Subject,
            Body = message.HtmlBody,
            IsBodyHtml = true
        };

        using var client = new SmtpClient(_options.Host, _options.Port)
        {
            EnableSsl = _options.EnableSsl,
            Credentials = new NetworkCredential(_options.UserName, _options.Password)  
        };

        await client.SendMailAsync(mail);
    }
}