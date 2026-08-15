using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AlimentationDongmo.Api.Services;

public class SmtpEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailSender> _logger;

    public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        var smtp = _configuration.GetSection("Smtp");
        var host = smtp["Host"];
        var user = smtp["User"];
        var password = smtp["Password"];
        var from = smtp["From"] ?? user;
        var port = int.Parse(smtp["Port"] ?? "587");

        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(password))
        {
            _logger.LogWarning("Configuration SMTP manquante : l'email vers {ToEmail} n'a pas été envoyé.", toEmail);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from!));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(user, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
