using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class VersementService : IVersementService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public VersementService(HttpClient http) => _http = http;

        public async Task<List<VersementViewModel>> GetAllAsync()
        {
            var r = await _http.GetAsync("api/Versement");
            if (!r.IsSuccessStatusCode) return new();
            var json = await r.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<VersementViewModel>>(json, _json) ?? new();
        }

        public async Task<VersementViewModel?> GetByIdAsync(int id)
        {
            var r = await _http.GetAsync($"api/Versement/{id}");
            if (!r.IsSuccessStatusCode) return null;
            var json = await r.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<VersementViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(VersementViewModel versement)
        {
            var content = new StringContent(JsonSerializer.Serialize(versement, _json), Encoding.UTF8, "application/json");
            var r = await _http.PostAsync("api/Versement", content);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, VersementViewModel versement)
        {
            var content = new StringContent(JsonSerializer.Serialize(versement, _json), Encoding.UTF8, "application/json");
            var r = await _http.PutAsync($"api/Versement/{id}", content);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var r = await _http.DeleteAsync($"api/Versement/{id}");
            return r.IsSuccessStatusCode;
        }
    }
}
