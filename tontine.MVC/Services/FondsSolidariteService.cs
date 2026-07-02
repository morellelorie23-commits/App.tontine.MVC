using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class FondsSolidariteService : IFondsSolidariteService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public FondsSolidariteService(HttpClient http) => _http = http;

        public async Task<List<FondsSolidariteViewModel>> GetAllAsync()
        {
            var resp = await _http.GetAsync("api/FondsSolidarite");
            if (!resp.IsSuccessStatusCode) return new();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<FondsSolidariteViewModel>>(json, _json) ?? new();
        }

        public async Task<FondsSolidariteViewModel?> GetByIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/FondsSolidarite/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<FondsSolidariteViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(FondsSolidariteViewModel fonds)
        {
            var body = new StringContent(JsonSerializer.Serialize(fonds), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("api/FondsSolidarite", body);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(FondsSolidariteViewModel fonds)
        {
            var body = new StringContent(JsonSerializer.Serialize(fonds), Encoding.UTF8, "application/json");
            var resp = await _http.PutAsync($"api/FondsSolidarite/{fonds.IdFonds}", body);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/FondsSolidarite/{id}");
            return resp.IsSuccessStatusCode;
        }
    }
}
