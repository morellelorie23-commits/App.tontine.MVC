using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class StatistiqueService : IStatistiqueService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public StatistiqueService(HttpClient http) => _http = http;

        public async Task<StatistiqueViewModel> GetStatsAsync()
        {
            try
            {
                var response = await _http.GetAsync("api/Statistique");
                if (!response.IsSuccessStatusCode) return new StatistiqueViewModel();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<StatistiqueViewModel>(json, _json)
                       ?? new StatistiqueViewModel();
            }
            catch
            {
                return new StatistiqueViewModel();
            }
        }

        public async Task<object?> GetStatsTontineAsync(int id)
        {
            try
            {
                var response = await _http.GetAsync($"api/Statistique/tontine/{id}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<object>(json, _json);
            }
            catch
            {
                return null;
            }
        }
    }
}
