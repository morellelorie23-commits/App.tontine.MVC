using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class TontineService : ITontineService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public TontineService(HttpClient http) => _http = http;

        public async Task<List<TontineViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/Tontine");
            if (!response.IsSuccessStatusCode) return new List<TontineViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TontineViewModel>>(json, _json)
                   ?? new List<TontineViewModel>();
        }

        public async Task<TontineViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/Tontine/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TontineViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(TontineViewModel tontine)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(tontine, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/Tontine", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, TontineViewModel tontine)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(tontine, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/Tontine/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Tontine/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}