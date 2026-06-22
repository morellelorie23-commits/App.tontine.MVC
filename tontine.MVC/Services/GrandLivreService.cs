using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class GrandLivreService : IGrandLivreService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public GrandLivreService(HttpClient http) => _http = http;

        public async Task<GrandLivreViewModel?> GetGrandLivreAsync(int idTontine, string? periode = null)
        {
            var url = $"api/GrandLivre/groupe/{idTontine}";
            if (!string.IsNullOrEmpty(periode)) url += $"?periode={periode}";
            var r = await _http.GetAsync(url);
            if (!r.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<GrandLivreViewModel>(await r.Content.ReadAsStringAsync(), _json);
        }

        public async Task<JournalViewModel?> GetJournalAsync(string codeJournal, string? periode = null)
        {
            var url = $"api/GrandLivre/journal/{codeJournal}";
            if (!string.IsNullOrEmpty(periode)) url += $"?periode={periode}";
            var r = await _http.GetAsync(url);
            if (!r.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<JournalViewModel>(await r.Content.ReadAsStringAsync(), _json);
        }

        public async Task<BalanceViewModel?> GetBalanceAsync(string? periode = null)
        {
            var url = "api/GrandLivre/balance";
            if (!string.IsNullOrEmpty(periode)) url += $"?periode={periode}";
            var r = await _http.GetAsync(url);
            if (!r.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<BalanceViewModel>(await r.Content.ReadAsStringAsync(), _json);
        }

        public async Task<RapprochementViewModel?> GetRapprochementAsync(int idTontine, decimal? soldeReelBanque = null)
        {
            var url = $"api/GrandLivre/rapprochement/{idTontine}";
            if (soldeReelBanque.HasValue) url += $"?soldeReelBanque={soldeReelBanque.Value}";
            var r = await _http.GetAsync(url);
            if (!r.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<RapprochementViewModel>(await r.Content.ReadAsStringAsync(), _json);
        }

        public async Task<JourneeComptableViewModel?> GetJourneeCouranteAsync()
        {
            var r = await _http.GetAsync("api/GrandLivre/journee/courante");
            if (!r.IsSuccessStatusCode) return null;
            var body = await r.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(body) || body == "null") return null;
            return JsonSerializer.Deserialize<JourneeComptableViewModel>(body, _json);
        }

        public async Task<JourneeComptableViewModel?> OuvrirJourneeAsync(DateOnly date, string ouvertPar)
        {
            var payload = new { dateJournee = date.ToString("yyyy-MM-dd"), ouvertPar };
            var r = await _http.PostAsync("api/GrandLivre/journee/ouvrir",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
            if (!r.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<JourneeComptableViewModel>(await r.Content.ReadAsStringAsync(), _json);
        }

        public async Task<bool> FermerJourneeAsync(int id)
        {
            var r = await _http.PostAsync($"api/GrandLivre/journee/fermer/{id}", null);
            return r.IsSuccessStatusCode;
        }

        public async Task<ReleveCaisseViewModel?> GetReleveCaisseAsync(string? compte, string? dateDebut, string? dateFin)
        {
            var url = "api/GrandLivre/releve-caisse";
            var qs = new List<string>();
            if (!string.IsNullOrEmpty(compte))    qs.Add($"compte={compte}");
            if (!string.IsNullOrEmpty(dateDebut)) qs.Add($"dateDebut={dateDebut}");
            if (!string.IsNullOrEmpty(dateFin))   qs.Add($"dateFin={dateFin}");
            if (qs.Any()) url += "?" + string.Join("&", qs);
            var r = await _http.GetAsync(url);
            if (!r.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<ReleveCaisseViewModel>(await r.Content.ReadAsStringAsync(), _json);
        }

        public async Task<BalanceDetailViewModel?> GetBalanceGeneraleAsync(string? dateDebut, string? dateFin)
        {
            var url = "api/GrandLivre/balance-generale";
            var qs = new List<string>();
            if (!string.IsNullOrEmpty(dateDebut)) qs.Add($"dateDebut={dateDebut}");
            if (!string.IsNullOrEmpty(dateFin))   qs.Add($"dateFin={dateFin}");
            if (qs.Any()) url += "?" + string.Join("&", qs);
            var r = await _http.GetAsync(url);
            if (!r.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<BalanceDetailViewModel>(await r.Content.ReadAsStringAsync(), _json);
        }

        public async Task<BalanceDetailViewModel?> GetBalanceClientAsync(string? dateDebut, string? dateFin, string? prefixeCompte)
        {
            var url = "api/GrandLivre/balance-client";
            var qs = new List<string>();
            if (!string.IsNullOrEmpty(dateDebut))     qs.Add($"dateDebut={dateDebut}");
            if (!string.IsNullOrEmpty(dateFin))       qs.Add($"dateFin={dateFin}");
            if (!string.IsNullOrEmpty(prefixeCompte)) qs.Add($"prefixeCompte={prefixeCompte}");
            if (qs.Any()) url += "?" + string.Join("&", qs);
            var r = await _http.GetAsync(url);
            if (!r.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<BalanceDetailViewModel>(await r.Content.ReadAsStringAsync(), _json);
        }

        public async Task<int?> CreerEcritureManuelleAsync(object payload)
        {
            var r = await _http.PostAsync("api/GrandLivre/ecriture-manuelle",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
            if (!r.IsSuccessStatusCode) return null;
            var d = JsonSerializer.Deserialize<JsonElement>(await r.Content.ReadAsStringAsync(), _json);
            return d.GetProperty("idEcriture").GetInt32();
        }

        public async Task<int?> TransfertCaisseAsync(object payload)
        {
            var r = await _http.PostAsync("api/GrandLivre/transfert-caisse",
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));
            if (!r.IsSuccessStatusCode) return null;
            var d = JsonSerializer.Deserialize<JsonElement>(await r.Content.ReadAsStringAsync(), _json);
            return d.GetProperty("idEcriture").GetInt32();
        }
    }
}
