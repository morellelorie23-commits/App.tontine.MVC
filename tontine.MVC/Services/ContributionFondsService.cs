using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class ContributionFondsService : IContributionFondsService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ContributionFondsService(HttpClient http) => _http = http;

        public async Task<List<ContributionFondsViewModel>> GetByFondsAsync(int idFonds)
        {
            var resp = await _http.GetAsync($"api/ContributionFonds/ByFonds/{idFonds}");
            if (!resp.IsSuccessStatusCode) return new();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ContributionFondsViewModel>>(json, _json) ?? new();
        }

        public async Task<bool> CreateAsync(ContributionFondsViewModel contribution)
        {
            var payload = new
            {
                contribution.IdFonds,
                contribution.IdMembre,
                contribution.Montant,
                contribution.Notes
            };
            var body = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var resp = await _http.PostAsync("api/ContributionFonds", body);
            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var resp = await _http.DeleteAsync($"api/ContributionFonds/{id}");
            return resp.IsSuccessStatusCode;
        }
    }
}
