using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Tijori.Application.Common;
using Tijori.Application.Interfaces.Services;

namespace Tijori.Infrastructure.Services;

public class EmailSettings
{
    public const string SectionName = "Email";

    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = "Tijori+";
    public bool EnableSsl { get; set; } = true;
}

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendOtpAsync(string email, string code, CancellationToken cancellationToken = default)
    {
        ValidateSettings();

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Your Tijori+ verification code";
        message.Body = new TextPart("plain")
        {
            Text = $"Your Tijori+ verification code is {code}. It expires in 1 minute."
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _settings.SmtpHost,
            _settings.SmtpPort,
            _settings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto,
            cancellationToken);
        await client.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        _logger.LogInformation("OTP email sent to {Email}", email);
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_settings.SmtpHost) ||
            string.IsNullOrWhiteSpace(_settings.FromAddress) ||
            string.IsNullOrWhiteSpace(_settings.Username) ||
            string.IsNullOrWhiteSpace(_settings.Password))
        {
            throw new AppException("Email service is not configured. Please set Email settings in appsettings.json.");
        }
    }
}

public class OtpDeliveryService : IOtpDeliveryService
{
    private readonly IEmailService _emailService;

    public OtpDeliveryService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public Task SendEmailOtpAsync(string email, string code, CancellationToken cancellationToken = default) =>
        _emailService.SendOtpAsync(email, code, cancellationToken);
}
