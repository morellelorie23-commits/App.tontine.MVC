using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class CycleService : ICycleService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };

        public CycleService(HttpClient http) => _http = http;

        public async Task<List<CycleViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/Cycle");
            if (!response.IsSuccessStatusCode) return new List<CycleViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CycleViewModel>>(json, _json)
                   ?? new List<CycleViewModel>();
        }

        public async Task<CycleViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/Cycle/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CycleViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(CycleViewModel cycle)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(cycle, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/Cycle", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, CycleViewModel cycle)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(cycle, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/Cycle/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Cycle/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}