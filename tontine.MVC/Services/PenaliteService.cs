using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class PenaliteService : IPenaliteService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PenaliteService(HttpClient http) => _http = http;

        public async Task<List<PenaliteViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/Penalite");
            if (!response.IsSuccessStatusCode) return new List<PenaliteViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PenaliteViewModel>>(json, _json)
                   ?? new List<PenaliteViewModel>();
        }

        public async Task<PenaliteViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/Penalite/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PenaliteViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(PenaliteViewModel penalite)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(penalite, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/Penalite", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, PenaliteViewModel penalite)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(penalite, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/Penalite/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Penalite/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}