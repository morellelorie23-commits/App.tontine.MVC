namespace tontine.MVC.Services
{
    public class SmsSettings
    {
        public string  Username { get; set; } = "sandbox";
        public string  ApiKey   { get; set; } = "";
        public string  From     { get; set; } = "TontineApp";
        public bool    Enabled  { get; set; } = false;
        public bool    Sandbox  { get; set; } = true;
    }

    public interface ISmsService
    {
        Task<bool> SendAsync(string telephone, string message);
        Task<bool> SendConfirmationPaiementAsync(string telephone, string nom, string tontine, decimal montant, DateTime date, int idCotisation);
        Task<bool> SendRappelCotisationAsync(string telephone, string nom, string tontine, decimal montant, DateTime dateLimite);
        Task<bool> SendConvocationAsync(string telephone, string nom, string objet, string tontine, DateTime dateReunion, string? lieu);
    }

    public class SmsService : ISmsService
    {
        private readonly SmsSettings       _settings;
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IConfiguration config, IHttpClientFactory factory, ILogger<SmsService> logger)
        {
            _settings = config.GetSection("SmsSettings").Get<SmsSettings>() ?? new SmsSettings();
            _factory  = factory;
            _logger   = logger;
        }

        // Normalise un numéro vers +237XXXXXXXXX
        private static string FormatPhone(string tel)
        {
            var t = tel.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
            if (t.StartsWith("+237")) return t;
            if (t.StartsWith("237"))  return "+" + t;
            // Numéro local camerounais : commence par 6 ou 2
            return "+237" + t;
        }

        public async Task<bool> SendAsync(string telephone, string message)
        {
            if (!_settings.Enabled || string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                _logger.LogWarning("SMS désactivé ou ApiKey manquante — configurer SmsSettings.");
                return false;
            }

            try
            {
                var baseUrl = _settings.Sandbox
                    ? "https://api.sandbox.africastalking.com"
                    : "https://api.africastalking.com";

                using var http = _factory.CreateClient();
                var req = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/version1/messaging");
                req.Headers.Add("apiKey", _settings.ApiKey);
                req.Headers.Add("Accept", "application/json");
                req.Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["username"] = _settings.Username,
                    ["to"]       = FormatPhone(telephone),
                    ["message"]  = message,
                    ["from"]     = _settings.From
                });

                var resp = await http.SendAsync(req);
                var body = await resp.Content.ReadAsStringAsync();
                _logger.LogInformation("SMS → {Phone} | HTTP {Code} | {Body}", FormatPhone(telephone), (int)resp.StatusCode, body);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur envoi SMS à {Phone}", telephone);
                return false;
            }
        }

        public Task<bool> SendConfirmationPaiementAsync(string telephone, string nom, string tontine, decimal montant, DateTime date, int idCotisation)
        {
            var msg = $"TontineApp ✅ Paiement recu\n" +
                      $"Bonjour {nom},\n" +
                      $"Votre cotisation de {montant:N0} FCFA pour {tontine} a ete enregistree le {date:dd/MM/yyyy}.\n" +
                      $"Ref: COT-{idCotisation:D5}. Merci !";
            return SendAsync(telephone, msg);
        }

        public Task<bool> SendRappelCotisationAsync(string telephone, string nom, string tontine, decimal montant, DateTime dateLimite)
        {
            var jours = (dateLimite.Date - DateTime.Today).Days;
            var urgence = jours <= 1 ? "URGENT" : "Rappel";
            var msg = $"TontineApp ⏰ {urgence}\n" +
                      $"Bonjour {nom},\n" +
                      $"Votre cotisation de {montant:N0} FCFA pour {tontine} est due dans {jours} jour(s) (le {dateLimite:dd/MM/yyyy}).\n" +
                      $"Payez avant la date pour eviter les penalites.";
            return SendAsync(telephone, msg);
        }

        public Task<bool> SendConvocationAsync(string telephone, string nom, string objet, string tontine, DateTime dateReunion, string? lieu)
        {
            var lieuStr = string.IsNullOrEmpty(lieu) ? "" : $"\nLieu : {lieu}";
            var msg = $"TontineApp 📅 Convocation\n" +
                      $"Bonjour {nom},\n" +
                      $"Reunion de {tontine} : {objet}\n" +
                      $"Date : {dateReunion:dd/MM/yyyy à HH:mm}{lieuStr}\n" +
                      $"Votre presence est importante.";
            return SendAsync(telephone, msg);
        }
    }
}
