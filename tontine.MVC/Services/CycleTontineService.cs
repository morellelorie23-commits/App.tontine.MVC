using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class CycleTontineService : ICycleTontineService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };

        public CycleTontineService(HttpClient http) => _http = http;

        public async Task<List<CycleTontineViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/CycleTontine");
            if (!response.IsSuccessStatusCode) return new List<CycleTontineViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CycleTontineViewModel>>(json, _json)
                   ?? new List<CycleTontineViewModel>();
        }

        public async Task<CycleTontineViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/CycleTontine/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CycleTontineViewModel>(json, _json);
        }

        public async Task<(bool Success, string? Error)> CreateWithErrorAsync(CycleTontineViewModel cycleTontine)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(cycleTontine, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/CycleTontine", content);
            if (response.IsSuccessStatusCode) return (true, null);
            var msg = await response.Content.ReadAsStringAsync();
            return (false, msg.Trim('"'));
        }

        public async Task<bool> CreateAsync(CycleTontineViewModel cycleTontine)
        {
            var (ok, _) = await CreateWithErrorAsync(cycleTontine);
            return ok;
        }

        public async Task<bool> UpdateAsync(int id, CycleTontineViewModel cycleTontine)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(cycleTontine, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/CycleTontine/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/CycleTontine/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<(int Crees, int Membres, string? Erreur)> RepartirAsync(int id)
        {
            var response = await _http.PostAsync($"api/CycleTontine/{id}/repartir", null);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                var crees   = doc.RootElement.TryGetProperty("crees",   out var c) ? c.GetInt32() : 0;
                var membres = doc.RootElement.TryGetProperty("membres", out var m) ? m.GetInt32() : 0;
                return (crees, membres, null);
            }
            var erreur = await response.Content.ReadAsStringAsync();
            return (0, 0, erreur.Trim('"'));
        }
    }
}
