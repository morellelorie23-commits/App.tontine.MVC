using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class MembreCycleTontineService : IMembreCycleTontineService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };

        public MembreCycleTontineService(HttpClient http) => _http = http;

        public async Task<List<MembreCycleTontineViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/MembreCycleTontine");
            if (!response.IsSuccessStatusCode) return new List<MembreCycleTontineViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MembreCycleTontineViewModel>>(json, _json)
                   ?? new List<MembreCycleTontineViewModel>();
        }

        public async Task<MembreCycleTontineViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/MembreCycleTontine/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MembreCycleTontineViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(MembreCycleTontineViewModel membreCycleTontine)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(membreCycleTontine, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/MembreCycleTontine", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, MembreCycleTontineViewModel membreCycleTontine)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(membreCycleTontine, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/MembreCycleTontine/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/MembreCycleTontine/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
