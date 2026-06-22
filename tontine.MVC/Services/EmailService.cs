using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace tontine.MVC.Services
{
    public class EmailSettings
    {
        public string Host { get; set; } = "";
        public int Port { get; set; } = 587;
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string FromName { get; set; } = "TontineApp";
        public string FromEmail { get; set; } = "";
        public bool UseSsl { get; set; } = false;
    }

    public interface IEmailService
    {
        Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlBody);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _settings = config.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
            _logger = logger;
        }

        public async Task<bool> SendAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            if (string.IsNullOrEmpty(_settings.Host) || string.IsNullOrEmpty(_settings.FromEmail))
            {
                _logger.LogWarning("Email non configuré — vérifiez EmailSettings dans appsettings.json");
                return false;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

                using var client = new SmtpClient();
                var secureOption = _settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;
                await client.ConnectAsync(_settings.Host, _settings.Port, secureOption);
                if (!string.IsNullOrEmpty(_settings.Username))
                    await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur envoi email à {Email}", toEmail);
                return false;
            }
        }
    }
}
