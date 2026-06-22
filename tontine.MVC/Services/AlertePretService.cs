using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IAlertePretService
    {
        Task<AlerteResult> EnvoyerAlertesPretsEnRetardAsync();
        Task<int> CompterPretsEnRetardAsync();
    }

    public class AlerteResult
    {
        public int PretsEnRetard { get; set; }
        public int EmailsEnvoyes { get; set; }
        public int EmailsEchoues { get; set; }
        public List<string> Details { get; set; } = new();
    }

    public class AlertePretService : IAlertePretService
    {
        private readonly HttpClient _http;
        private readonly IEmailService _emailService;
        private readonly ILogger<AlertePretService> _logger;
        private readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public AlertePretService(HttpClient http, IEmailService emailService, ILogger<AlertePretService> logger)
        {
            _http = http;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<AlerteResult> EnvoyerAlertesPretsEnRetardAsync()
        {
            var result = new AlerteResult();

            try
            {
                // Récupérer tous les prêts en retard
                var r = await _http.GetAsync("api/Pret");
                if (!r.IsSuccessStatusCode) return result;

                var prets = JsonSerializer.Deserialize<List<PretViewModel>>(
                    await r.Content.ReadAsStringAsync(), _opts) ?? new();

                var pretsEnRetard = prets.Where(p => p.Statut == "En retard").ToList();
                result.PretsEnRetard = pretsEnRetard.Count;

                // Récupérer tous les membres
                var rm = await _http.GetAsync("api/Membre");
                var membres = new List<MembreViewModel>();
                if (rm.IsSuccessStatusCode)
                    membres = JsonSerializer.Deserialize<List<MembreViewModel>>(
                        await rm.Content.ReadAsStringAsync(), _opts) ?? new();

                foreach (var pret in pretsEnRetard)
                {
                    var membre = membres.FirstOrDefault(m => m.IdMembre == pret.IdMembre);
                    if (membre == null || string.IsNullOrEmpty(membre.Email))
                    {
                        result.Details.Add($"Prêt #{pret.IdPret} — {pret.NomMembre} : pas d'email");
                        result.EmailsEchoues++;
                        continue;
                    }

                    var sujet = $"[TontineApp] Rappel : Prêt en retard — {pret.Montant:N0} FCFA";
                    var corps = BuildEmailBody(membre, pret);

                    var ok = await _emailService.SendAsync(membre.Email, $"{membre.Nom} {membre.Prenom}", sujet, corps);
                    if (ok)
                    {
                        result.EmailsEnvoyes++;
                        result.Details.Add($"Prêt #{pret.IdPret} — {pret.NomMembre} : email envoyé à {membre.Email}");
                    }
                    else
                    {
                        result.EmailsEchoues++;
                        result.Details.Add($"Prêt #{pret.IdPret} — {pret.NomMembre} : échec envoi");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des alertes prêts");
            }

            return result;
        }

        public async Task<int> CompterPretsEnRetardAsync()
        {
            try
            {
                var r = await _http.GetAsync("api/Pret");
                if (!r.IsSuccessStatusCode) return 0;
                var prets = JsonSerializer.Deserialize<List<PretViewModel>>(
                    await r.Content.ReadAsStringAsync(), _opts) ?? new();
                return prets.Count(p => p.Statut == "En retard");
            }
            catch { return 0; }
        }

        private string BuildEmailBody(MembreViewModel membre, PretViewModel pret)
        {
            var echeance = pret.DateRemboursement == DateTime.MinValue ? "non définie" : pret.DateRemboursement.ToString("dd/MM/yyyy");
            var retard = pret.DateRemboursement != DateTime.MinValue
                ? $"{(DateTime.Now - pret.DateRemboursement).Days} jour(s)"
                : "indéterminé";

            return $"""
            <!DOCTYPE html>
            <html>
            <head><meta charset="utf-8"></head>
            <body style="font-family:Arial,sans-serif; background:#F5F5F5; margin:0; padding:20px;">
              <div style="max-width:520px; margin:0 auto; background:white; border-radius:12px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.1);">
                <div style="background:#C62828; padding:24px 28px;">
                  <div style="color:white; font-size:20px; font-weight:700;">TontineApp</div>
                  <div style="color:rgba(255,255,255,0.85); font-size:13px; margin-top:4px;">Alerte — Prêt en retard</div>
                </div>
                <div style="padding:28px;">
                  <p style="font-size:15px; color:#1a1a1a;">Bonjour <strong>{membre.Nom} {membre.Prenom}</strong>,</p>
                  <p style="color:#444; line-height:1.6;">
                    Nous vous informons que votre prêt est actuellement <strong style="color:#C62828;">en retard de remboursement</strong>.
                    Veuillez régulariser votre situation dans les meilleurs délais.
                  </p>
                  <div style="background:#FFF5F5; border-left:4px solid #C62828; border-radius:8px; padding:16px 20px; margin:20px 0;">
                    <div style="font-size:13px; color:#888; margin-bottom:6px;">DÉTAILS DU PRÊT</div>
                    <div style="display:flex; justify-content:space-between; margin-bottom:4px;">
                      <span style="color:#444;">Montant emprunté</span>
                      <strong style="color:#C62828;">{pret.Montant:N0} FCFA</strong>
                    </div>
                    <div style="display:flex; justify-content:space-between; margin-bottom:4px;">
                      <span style="color:#444;">Date d'échéance</span>
                      <strong>{echeance}</strong>
                    </div>
                    <div style="display:flex; justify-content:space-between;">
                      <span style="color:#444;">Retard</span>
                      <strong style="color:#C62828;">{retard}</strong>
                    </div>
                  </div>
                  <p style="color:#444; font-size:13px; line-height:1.6;">
                    Sans régularisation, votre garant (répondant) sera contacté conformément aux règles de la tontine.
                  </p>
                  <p style="color:#888; font-size:12px; margin-top:24px; padding-top:16px; border-top:1px solid #EEE;">
                    Ce message est envoyé automatiquement par TontineApp. Ne pas répondre à cet email.
                  </p>
                </div>
              </div>
            </body>
            </html>
            """;
        }
    }
}
