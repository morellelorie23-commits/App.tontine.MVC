using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IPaiementMobileService
    {
        Task<(bool Succes, string Reference, string Message)> InitierAsync(InitierPaiementViewModel req);
        Task<PaiementMobileViewModel?> GetStatutAsync(string reference);
        Task<List<PaiementMobileViewModel>> GetByCotisationAsync(int idCotisation);
        Task<bool> SimulerAsync(string reference, bool succes = true);
    }

    public class PaiementMobileService : IPaiementMobileService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public PaiementMobileService(HttpClient http) => _http = http;

        public async Task<(bool Succes, string Reference, string Message)> InitierAsync(InitierPaiementViewModel req)
        {
            var payload = new { req.IdCotisation, req.Telephone, req.Operateur, req.Montant };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("api/PaiementMobile/initier", content);
            if (!response.IsSuccessStatusCode) return (false, "", "Erreur lors de l'initiation du paiement.");
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            return (true,
                root.TryGetProperty("reference", out var r) ? r.GetString()! : "",
                root.TryGetProperty("message",   out var m) ? m.GetString()! : "Demande envoyée.");
        }

        public async Task<PaiementMobileViewModel?> GetStatutAsync(string reference)
        {
            var response = await _http.GetAsync($"api/PaiementMobile/statut/{reference}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PaiementMobileViewModel>(json, _json);
        }

        public async Task<List<PaiementMobileViewModel>> GetByCotisationAsync(int idCotisation)
        {
            var response = await _http.GetAsync($"api/PaiementMobile/cotisation/{idCotisation}");
            if (!response.IsSuccessStatusCode) return new List<PaiementMobileViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PaiementMobileViewModel>>(json, _json) ?? new();
        }

        public async Task<bool> SimulerAsync(string reference, bool succes = true)
        {
            var response = await _http.PostAsync($"api/PaiementMobile/simuler/{reference}?succes={succes}", null);
            return response.IsSuccessStatusCode;
        }
    }
}
