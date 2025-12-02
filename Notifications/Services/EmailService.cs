
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Notifications.Services;

public class EmailService(IConfiguration configuration, ILogger<EmailService> logger) : IEmailService
{   
    public async Task SendWelcomeEmailAsync(string toEmail, string? firstName, CancellationToken cancellationToken = default)
    {
        var displayName = firstName ?? toEmail.Split('@')[0];

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            configuration["Email:FromName"] ?? "Re-Sports",
            configuration["Email:FromAddress"] ?? "noreply@re-sports.local"));
        message.To.Add(new MailboxAddress(displayName, toEmail));
        message.Subject = "Welcome to re-sports!";

        message.Body = new TextPart("html")
        {
            Text = $"""
                <html>
                <body style="font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;">
                    <h1 style="color: #333;">Welcome to Re-Sports!</h1>
                    <p>Hi {displayName},</p>
                    <p>Thank you for registering with Re-Sports. Your account has been successfully created.</p>
                    <p>You can now log in and start using our platform.</p>
                    <br/>
                    <p>Best regards,<br/>The Re-Sports Team</p>
                </body>
                </html>
                """
        };

        await SendEmailAsync(message, cancellationToken);

        logger.LogInformation("Welcome email sent to {Email}", toEmail);
    }

    private async Task SendEmailAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        var smtpHost = configuration["Email:SmtpHost"] ?? "localhost";
        var smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "1025");

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(smtpHost, smtpPort, false, cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", message.To);
            throw;
        }
    }
}
