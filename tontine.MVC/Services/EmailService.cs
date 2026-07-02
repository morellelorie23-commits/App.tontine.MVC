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
        Task<bool> SendConfirmationPaiementAsync(string toEmail, string nom, string tontine, string cycle, decimal montant, DateTime date, int idCotisation);
        Task<bool> SendRappelCotisationAsync(string toEmail, string nom, string tontine, decimal montant, DateTime dateLimite);
        Task<bool> SendConvocationAsync(string toEmail, string nom, string objet, string tontine, DateTime dateReunion, string? lieu, string? notes);
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

        public Task<bool> SendConfirmationPaiementAsync(string toEmail, string nom, string tontine, string cycle, decimal montant, DateTime date, int idCotisation)
        {
            var html = $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:auto;"">
  <div style=""background:#0F6E56;padding:28px 32px;"">
    <h1 style=""color:white;margin:0;font-size:22px;"">✅ Paiement confirmé</h1>
    <p style=""color:rgba(255,255,255,0.8);margin:6px 0 0;font-size:14px;"">Reçu N° {idCotisation:D5}</p>
  </div>
  <div style=""background:white;padding:28px 32px;border:1px solid #E0E0E0;"">
    <p style=""font-size:15px;color:#333;"">Bonjour <strong>{nom}</strong>,</p>
    <p style=""color:#555;font-size:14px;"">Votre cotisation a bien été enregistrée. Voici le récapitulatif :</p>
    <div style=""background:#F4FDF9;border-radius:10px;padding:20px;margin:20px 0;"">
      <table style=""width:100%;font-size:14px;color:#333;"">
        <tr><td style=""padding:6px 0;color:#888;"">Tontine</td><td style=""font-weight:600;text-align:right;"">{tontine}</td></tr>
        <tr><td style=""padding:6px 0;color:#888;"">Cycle</td><td style=""font-weight:600;text-align:right;"">{cycle}</td></tr>
        <tr><td style=""padding:6px 0;color:#888;"">Date</td><td style=""font-weight:600;text-align:right;"">{date:dd/MM/yyyy}</td></tr>
        <tr style=""border-top:2px solid #0F6E56;"">
          <td style=""padding-top:12px;font-size:15px;font-weight:700;color:#0F6E56;"">Montant</td>
          <td style=""padding-top:12px;font-size:18px;font-weight:800;color:#0F6E56;text-align:right;"">{montant:N0} FCFA</td>
        </tr>
      </table>
    </div>
    <p style=""color:#888;font-size:12px;"">Merci pour votre ponctualité. Ce message est un accusé de réception automatique.</p>
  </div>
  <div style=""background:#F5F5F5;padding:14px 32px;text-align:center;"">
    <p style=""color:#aaa;font-size:11px;margin:0;"">TontineApp — Ne pas répondre à cet email</p>
  </div>
</div>";
            return SendAsync(toEmail, nom, $"✅ Cotisation reçue — {montant:N0} FCFA ({tontine})", html);
        }

        public Task<bool> SendRappelCotisationAsync(string toEmail, string nom, string tontine, decimal montant, DateTime dateLimite)
        {
            var joursRestants = (dateLimite.Date - DateTime.Today).Days;
            var urgence = joursRestants <= 1 ? "#C62828" : "#F57C00";
            var html = $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:auto;"">
  <div style=""background:{urgence};padding:28px 32px;"">
    <h1 style=""color:white;margin:0;font-size:22px;"">⏰ Rappel de cotisation</h1>
    <p style=""color:rgba(255,255,255,0.85);margin:6px 0 0;font-size:14px;"">Échéance dans {joursRestants} jour(s)</p>
  </div>
  <div style=""background:white;padding:28px 32px;border:1px solid #E0E0E0;"">
    <p style=""font-size:15px;color:#333;"">Bonjour <strong>{nom}</strong>,</p>
    <p style=""color:#555;font-size:14px;"">Votre cotisation pour la tontine <strong>{tontine}</strong> arrive bientôt à échéance.</p>
    <div style=""background:#FFF8E1;border-left:4px solid {urgence};border-radius:6px;padding:16px 20px;margin:20px 0;"">
      <div style=""font-size:13px;color:#888;"">Montant dû</div>
      <div style=""font-size:24px;font-weight:800;color:{urgence};""> {montant:N0} FCFA</div>
      <div style=""font-size:13px;color:#555;margin-top:4px;"">Date limite : <strong>{dateLimite:dd/MM/yyyy}</strong></div>
    </div>
    <p style=""color:#888;font-size:12px;"">Évitez les pénalités en réglant avant la date limite. Contactez votre gestionnaire pour tout paiement.</p>
  </div>
  <div style=""background:#F5F5F5;padding:14px 32px;text-align:center;"">
    <p style=""color:#aaa;font-size:11px;margin:0;"">TontineApp — Ne pas répondre à cet email</p>
  </div>
</div>";
            return SendAsync(toEmail, nom, $"⏰ Rappel cotisation — {tontine} — échéance le {dateLimite:dd/MM/yyyy}", html);
        }

        public Task<bool> SendConvocationAsync(string toEmail, string nom, string objet, string tontine, DateTime dateReunion, string? lieu, string? notes)
        {
            var lieuLine  = string.IsNullOrEmpty(lieu)  ? "" : $"<tr><td style=\"padding:6px 0;color:#888;\">Lieu</td><td style=\"font-weight:600;\">{lieu}</td></tr>";
            var notesLine = string.IsNullOrEmpty(notes) ? "" : $"<tr><td style=\"padding:6px 0;color:#888;\">Notes</td><td style=\"color:#555;\">{notes}</td></tr>";
            var html = $@"
<div style=""font-family:Arial,sans-serif;max-width:560px;margin:auto;"">
  <div style=""background:#1565C0;padding:28px 32px;"">
    <h1 style=""color:white;margin:0;font-size:22px;"">Convocation — Réunion</h1>
    <p style=""color:rgba(255,255,255,0.85);margin:6px 0 0;font-size:14px;"">Tontine : {tontine}</p>
  </div>
  <div style=""background:white;padding:28px 32px;border:1px solid #E0E0E0;"">
    <p style=""font-size:15px;color:#333;"">Bonjour <strong>{nom}</strong>,</p>
    <p style=""color:#555;font-size:14px;"">Vous êtes convoqué(e) à la prochaine réunion de votre tontine :</p>
    <div style=""background:#EEF4FF;border-left:4px solid #1565C0;border-radius:6px;padding:16px 20px;margin:20px 0;"">
      <table style=""width:100%;font-size:14px;color:#333;"">
        <tr><td style=""padding:6px 0;color:#888;"">Objet</td><td style=""font-weight:600;"">{objet}</td></tr>
        <tr><td style=""padding:6px 0;color:#888;"">Date</td><td style=""font-weight:600;"">{dateReunion:dd/MM/yyyy à HH:mm}</td></tr>
        {lieuLine}{notesLine}
      </table>
    </div>
    <p style=""color:#888;font-size:12px;"">Votre présence est importante. En cas d'absence, veuillez prévenir votre gestionnaire.</p>
  </div>
  <div style=""background:#F5F5F5;padding:14px 32px;text-align:center;"">
    <p style=""color:#aaa;font-size:11px;margin:0;"">TontineApp — Ne pas répondre à cet email</p>
  </div>
</div>";
            return SendAsync(toEmail, nom, $"Convocation — Réunion {tontine} — {dateReunion:dd/MM/yyyy}", html);
        }
    }
}
