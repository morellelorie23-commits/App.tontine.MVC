using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class JournalService : IJournalService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _opts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public JournalService(HttpClient http) => _http = http;

        public async Task<List<JournalActiviteViewModel>> GetAllAsync()
        {
            var res = await _http.GetAsync("api/JournalActivite");
            if (!res.IsSuccessStatusCode) return new List<JournalActiviteViewModel>();
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<JournalActiviteViewModel>>(json, _opts)
                   ?? new List<JournalActiviteViewModel>();
        }

        public async Task<List<JournalActiviteViewModel>> GetRecentAsync(int count = 10)
        {
            var all = await GetAllAsync();
            return all.Take(count).ToList();
        }
    }
}
