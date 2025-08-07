using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MimeKit;
using EndPointEcommerce.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EndPointEcommerce.Infrastructure.Services;

public class MailData
{
    public required string To { get; set; }
    public required string ToName { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
}

public interface IMailer
{
    Task<bool> SendMailAsync(MailData mailData);
}

public class Mailer : IMailer
{
    private readonly ISmtpClient _smtpClient;
    private readonly MailSettings _mailSettings;
    private readonly string[] _mailCcAddresses;
    private readonly string[] _mailBccAddresses;
    private readonly ILogger<Mailer> _logger;

    public Mailer(
        ISmtpClient smtpClient,
        IOptions<MailSettings> mailSettingsOptions,
        IConfiguration config,
        ILogger<Mailer> logger
    ) {
        _smtpClient = smtpClient;
        _mailSettings = mailSettingsOptions.Value;
        _mailCcAddresses = SplitAddresses(config["MailCcAddresses"]);
        _mailBccAddresses = SplitAddresses(config["MailBccAddresses"]);
        _logger = logger;
    }

    private static string[] SplitAddresses(string? adresses) =>
        adresses?.Split(";", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

    public async Task<bool> SendMailAsync(MailData mailData)
    {
        try
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(_mailSettings.SenderName, _mailSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress(mailData.ToName, mailData.To));

            emailMessage.Cc.AddRange(_mailCcAddresses.Select(a => new MailboxAddress(a, a)));
            emailMessage.Bcc.AddRange(_mailBccAddresses.Select(a => new MailboxAddress(a, a)));

            emailMessage.Subject = mailData.Subject;

            var emailBodyBuilder = new BodyBuilder
            {
                TextBody = mailData.Body,
                HtmlBody = mailData.Body
            };

            emailMessage.Body = emailBodyBuilder.ToMessageBody();

            if (String.IsNullOrEmpty(_mailSettings.Server))
            {
                _logger.LogWarning($"No mail server configured, not sending email. - To: {emailMessage.To}, Subject: {emailMessage.Subject}");
                _logger.LogDebug($"\nFrom: {emailMessage.From}\nTo: {emailMessage.To}\nCc: {emailMessage.Cc}\nBcc: {emailMessage.Bcc}\nSubject: {emailMessage.Subject}\nBody: {emailMessage.Body}");
            }
            else
            {
                await _smtpClient.ConnectAsync(_mailSettings.Server, _mailSettings.Port,
                    MailKit.Security.SecureSocketOptions.StartTls);
                if (!String.IsNullOrEmpty(_mailSettings.UserName) || !String.IsNullOrEmpty(_mailSettings.Password))
                    await _smtpClient.AuthenticateAsync(_mailSettings.UserName, _mailSettings.Password);
                await _smtpClient.SendAsync(emailMessage);
                await _smtpClient.DisconnectAsync(true);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", mailData.To);
            return false;
        }
    }
}