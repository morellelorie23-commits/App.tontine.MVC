using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class DemandeAideService : IDemandeAideService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public DemandeAideService(HttpClient http) => _http = http;

        public async Task<List<DemandeAideViewModel>> GetByFondsAsync(int idFonds)
        {
            var resp = await _http.GetAsync($"api/DemandeAide/ByFonds/{idFonds}");
            if (!resp.IsSuccessStatusCode) return new();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<DemandeAideViewModel>>(json, _json) ?? new();
        }

        public async Task<bool> CreateAsync(DemandeAideViewModel demande)
        {
            var payload = new
            {
                demande.IdFonds,
                demande.IdMembre,
                demande.MontantDemande,
                demande.Motif
            };
            var body = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("api/DemandeAide", body);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> ApprouverAsync(int id, decimal montantAccorde, string? notes)
        {
            var payload = new { MontantAccorde = montantAccorde, Notes = notes };
            var body = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync($"api/DemandeAide/{id}/Approuver", body);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> RejeterAsync(int id, string? notes)
        {
            var body = new StringContent(JsonSerializer.Serialize(notes ?? ""), Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync($"api/DemandeAide/{id}/Rejeter", body);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> MarquerPayeAsync(int id)
        {
            var resp = await _http.PutAsync($"api/DemandeAide/{id}/MarquerPaye",
                new StringContent("", Encoding.UTF8, "application/json"));
            return resp.IsSuccessStatusCode;
        }
    }
}
