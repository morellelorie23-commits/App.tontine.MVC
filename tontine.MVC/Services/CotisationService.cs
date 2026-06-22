using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class CotisationService : ICotisationService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public CotisationService(HttpClient http) => _http = http;

        public async Task<List<CotisationViewModel>> GetAllAsync()
        {
            var r = await _http.GetAsync("api/Cotisation");
            if (!r.IsSuccessStatusCode) return new();
            var json = await r.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CotisationViewModel>>(json, _json) ?? new();
        }

        public async Task<CotisationViewModel?> GetByIdAsync(int id)
        {
            var r = await _http.GetAsync($"api/Cotisation/{id}");
            if (!r.IsSuccessStatusCode) return null;
            var json = await r.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CotisationViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(CotisationViewModel cotisation)
        {
            var content = new StringContent(JsonSerializer.Serialize(cotisation, _json), Encoding.UTF8, "application/json");
            var r = await _http.PostAsync("api/Cotisation", content);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, CotisationViewModel cotisation)
        {
            var content = new StringContent(JsonSerializer.Serialize(cotisation, _json), Encoding.UTF8, "application/json");
            var r = await _http.PutAsync($"api/Cotisation/{id}", content);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var r = await _http.DeleteAsync($"api/Cotisation/{id}");
            return r.IsSuccessStatusCode;
        }

        public async Task<int> AppliquerPenalitesAsync()
        {
            var r = await _http.PostAsync("api/Cotisation/appliquerpenalites", null);
            if (!r.IsSuccessStatusCode) return 0;
            var json = await r.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("misAJour", out var v) ? v.GetInt32() : 0;
        }

        public async Task<bool> PayerCashAsync(int id)
        {
            var r = await _http.PostAsync($"api/Cotisation/{id}/payer-cash", null);
            return r.IsSuccessStatusCode;
        }

        public async Task<List<TourViewModel>> GetToursAsync(int idCycle, int idTontine)
        {
            var r = await _http.GetAsync($"api/MembreCycleTontine/tours/{idCycle}/{idTontine}");
            if (!r.IsSuccessStatusCode) return new();
            var json = await r.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TourViewModel>>(json, _json) ?? new();
        }

        public async Task<LivreDeCompteViewModel?> GetLivreDeCompteAsync(int idCycle, int idTontine)
        {
            var r = await _http.GetAsync($"api/Cotisation/livre/{idCycle}/{idTontine}");
            if (!r.IsSuccessStatusCode) return null;
            var json = await r.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<LivreDeCompteViewModel>(json, _json);
        }
    }
}
