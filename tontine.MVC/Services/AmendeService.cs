using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class AmendeService : IAmendeService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };

        public AmendeService(HttpClient http) => _http = http;

        public async Task<List<AmendeViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/Amende");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<AmendeViewModel>>(json, _json) ?? new();
        }

        public async Task<AmendeViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/Amende/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AmendeViewModel>(json, _json);
        }

        public async Task<int> GenererAsync(int cycleId)
        {
            var response = await _http.PostAsync($"api/Amende/generer/{cycleId}", null);
            if (!response.IsSuccessStatusCode) return 0;
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(json);
            return result.TryGetProperty("generees", out var prop) ? prop.GetInt32() : 0;
        }

        public async Task<bool> MarquerPayeeAsync(int id)
        {
            var response = await _http.PostAsync($"api/Amende/{id}/payer", null);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Amende/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
