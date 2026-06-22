using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class MembrePosteCycleService : IMembrePosteCycleService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };

        public MembrePosteCycleService(HttpClient http) => _http = http;

        public async Task<List<MembrePosteCycleViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/MembrePosteCycle");
            if (!response.IsSuccessStatusCode) return new List<MembrePosteCycleViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MembrePosteCycleViewModel>>(json, _json)
                   ?? new List<MembrePosteCycleViewModel>();
        }

        public async Task<MembrePosteCycleViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/MembrePosteCycle/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MembrePosteCycleViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(MembrePosteCycleViewModel membrePosteCycle)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(membrePosteCycle, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/MembrePosteCycle", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, MembrePosteCycleViewModel membrePosteCycle)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(membrePosteCycle, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/MembrePosteCycle/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/MembrePosteCycle/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
