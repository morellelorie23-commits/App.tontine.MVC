using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class PosteService : IPosteService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PosteService(HttpClient http) => _http = http;

        public async Task<List<PosteViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/Poste");
            if (!response.IsSuccessStatusCode) return new List<PosteViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PosteViewModel>>(json, _json)
                   ?? new List<PosteViewModel>();
        }

        public async Task<PosteViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/Poste/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PosteViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(PosteViewModel poste)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(poste, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/Poste", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, PosteViewModel poste)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(poste, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/Poste/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Poste/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}