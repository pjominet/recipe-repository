using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using RecipeRandomizer.Business.Utils.Settings;
using RecipeRepository.Logic.Interfaces;

namespace RecipeRepository.Logic.Services;

public class EmailService(IOptions<EmailSettings> emailSettings) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendEmailAsync(string to, string subject, string html, string? sender = null)
    {
        try
        {
            // create message
            var email = new MimeMessage
            {
                Sender = MailboxAddress.Parse(sender ?? _emailSettings.Sender)
            };
            email.From.Add(new MailboxAddress(_emailSettings.SenderName, sender ?? _emailSettings.Sender));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = html
            };

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpKey);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception e)
        {
            throw new InvalidOperationException(e.Message);
        }
    }
}
