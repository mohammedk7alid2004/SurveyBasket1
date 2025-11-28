using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;
using SurveyBasket.BLL.Settings;

namespace SurveyBasket.BLL.Services;

public class EmailService(IOptions<MailSettings>mailSettings) : IEmailSender
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var Message = new MimeMessage
        {
            Sender=MailboxAddress.Parse(_mailSettings.Email),
            Subject=subject,
        };
        Message.To.Add(MailboxAddress.Parse(email));
        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };
        Message.Body = builder.ToMessageBody();
        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        smtp.Authenticate(_mailSettings.Email, _mailSettings.AppPassword);
        await smtp.SendAsync(Message);
        smtp.Disconnect(true);
    }
}
