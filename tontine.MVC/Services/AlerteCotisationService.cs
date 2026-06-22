using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IAlerteCotisationService
    {
        Task<AlerteCotisationResult> EnvoyerAlertesCotisationsEnRetardAsync();
        Task<int> CompterCotisationsEnRetardAsync();
    }

    public class AlerteCotisationResult
    {
        public int CotisationsEnRetard { get; set; }
        public int EmailsEnvoyes       { get; set; }
        public int EmailsEchoues       { get; set; }
        public List<string> Details    { get; set; } = new();
    }

    public class AlerteCotisationService : IAlerteCotisationService
    {
        private readonly HttpClient _http;
        private readonly IEmailService _email;
        private readonly ILogger<AlerteCotisationService> _logger;
        private readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public AlerteCotisationService(HttpClient http, IEmailService email, ILogger<AlerteCotisationService> logger)
        {
            _http   = http;
            _email  = email;
            _logger = logger;
        }

        public async Task<AlerteCotisationResult> EnvoyerAlertesCotisationsEnRetardAsync()
        {
            var result = new AlerteCotisationResult();
            try
            {
                var rc = await _http.GetAsync("api/Cotisation");
                if (!rc.IsSuccessStatusCode) return result;
                var cotisations = JsonSerializer.Deserialize<List<CotisationViewModel>>(
                    await rc.Content.ReadAsStringAsync(), _opts) ?? new();

                var enRetard = cotisations.Where(c => c.Statut == "En retard").ToList();
                result.CotisationsEnRetard = enRetard.Count;

                var rm = await _http.GetAsync("api/Membre");
                var membres = new List<MembreViewModel>();
                if (rm.IsSuccessStatusCode)
                    membres = JsonSerializer.Deserialize<List<MembreViewModel>>(
                        await rm.Content.ReadAsStringAsync(), _opts) ?? new();

                foreach (var cot in enRetard)
                {
                    var membre = membres.FirstOrDefault(m => m.IdMembre == cot.IdMembre);
                    if (membre == null || string.IsNullOrEmpty(membre.Email))
                    {
                        result.Details.Add($"Cotisation #{cot.IdCotisation} — {cot.NomMembre} : pas d'email");
                        result.EmailsEchoues++;
                        continue;
                    }

                    var sujet = $"[TontineApp] Rappel : Cotisation en retard — {cot.LibelleTontine}";
                    var corps = BuildEmail(membre, cot);
                    var ok = await _email.SendAsync(membre.Email, $"{membre.Nom} {membre.Prenom}", sujet, corps);
                    if (ok)
                    {
                        result.EmailsEnvoyes++;
                        result.Details.Add($"{cot.NomMembre} : email envoyé ({cot.Montant:N0} FCFA — {cot.LibelleTontine})");
                    }
                    else
                    {
                        result.EmailsEchoues++;
                        result.Details.Add($"{cot.NomMembre} : échec envoi");
                    }
                }
            }
            catch (Exception ex) { _logger.LogError(ex, "Erreur alertes cotisations"); }
            return result;
        }

        public async Task<int> CompterCotisationsEnRetardAsync()
        {
            try
            {
                var rc = await _http.GetAsync("api/Cotisation");
                if (!rc.IsSuccessStatusCode) return 0;
                var cotisations = JsonSerializer.Deserialize<List<CotisationViewModel>>(
                    await rc.Content.ReadAsStringAsync(), _opts) ?? new();
                return cotisations.Count(c => c.Statut == "En retard");
            }
            catch { return 0; }
        }

        private string BuildEmail(MembreViewModel m, CotisationViewModel c) => $"""
            <!DOCTYPE html><html><head><meta charset="utf-8"></head>
            <body style="font-family:Arial,sans-serif;background:#F5F5F5;margin:0;padding:20px;">
              <div style="max-width:520px;margin:0 auto;background:white;border-radius:12px;overflow:hidden;box-shadow:0 2px 8px rgba(0,0,0,0.1);">
                <div style="background:#E65100;padding:24px 28px;">
                  <div style="color:white;font-size:20px;font-weight:700;">TontineApp</div>
                  <div style="color:rgba(255,255,255,0.85);font-size:13px;margin-top:4px;">Rappel — Cotisation en retard</div>
                </div>
                <div style="padding:28px;">
                  <p style="font-size:15px;color:#1a1a1a;">Bonjour <strong>{m.Nom} {m.Prenom}</strong>,</p>
                  <p style="color:#444;line-height:1.6;">Votre cotisation est actuellement <strong style="color:#E65100;">en retard</strong>. Merci de régulariser votre situation dès que possible.</p>
                  <div style="background:#FFF3E0;border-left:4px solid #E65100;border-radius:8px;padding:16px 20px;margin:20px 0;">
                    <div style="font-size:13px;color:#888;margin-bottom:6px;">DÉTAILS</div>
                    <div style="margin-bottom:4px;"><span style="color:#444;">Tontine</span> — <strong>{c.LibelleTontine}</strong></div>
                    <div style="margin-bottom:4px;"><span style="color:#444;">Cycle</span> — <strong>{c.NomCycle}</strong></div>
                    <div style="margin-bottom:4px;"><span style="color:#444;">Montant dû</span> — <strong style="color:#E65100;">{c.Montant:N0} FCFA</strong></div>
                    <div><span style="color:#444;">Date prévue</span> — <strong>{c.DateCotisation:dd/MM/yyyy}</strong></div>
                  </div>
                  <p style="color:#888;font-size:12px;margin-top:24px;padding-top:16px;border-top:1px solid #EEE;">Message automatique TontineApp — Ne pas répondre.</p>
                </div>
              </div>
            </body></html>
            """;
    }
}
