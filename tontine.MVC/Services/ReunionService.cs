using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class ReunionService : IReunionService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };

        public ReunionService(HttpClient http) => _http = http;

        public async Task<List<ReunionViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/Reunion");
            if (!response.IsSuccessStatusCode) return new List<ReunionViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ReunionViewModel>>(json, _json) ?? new();
        }

        public async Task<ReunionViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/Reunion/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ReunionViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(ReunionViewModel vm)
        {
            var content = new StringContent(JsonSerializer.Serialize(vm, _json), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("api/Reunion", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, ReunionViewModel vm)
        {
            var content = new StringContent(JsonSerializer.Serialize(vm, _json), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"api/Reunion/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Reunion/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
