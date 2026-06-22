using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class CycleTontinePenaliteService : ICycleTontinePenaliteService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };

        public CycleTontinePenaliteService(HttpClient http) => _http = http;

        public async Task<List<CycleTontinePenaliteViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/CycleTontinePenalite");
            if (!response.IsSuccessStatusCode) return new List<CycleTontinePenaliteViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CycleTontinePenaliteViewModel>>(json, _json)
                   ?? new List<CycleTontinePenaliteViewModel>();
        }

        public async Task<CycleTontinePenaliteViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/CycleTontinePenalite/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CycleTontinePenaliteViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(CycleTontinePenaliteViewModel cycleTontinePenalite)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(cycleTontinePenalite, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/CycleTontinePenalite", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, CycleTontinePenaliteViewModel cycleTontinePenalite)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(cycleTontinePenalite, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/CycleTontinePenalite/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/CycleTontinePenalite/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
