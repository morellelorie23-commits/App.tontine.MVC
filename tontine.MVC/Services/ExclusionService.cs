using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class ExclusionService : IExclusionService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public ExclusionService(HttpClient http) => _http = http;

        public async Task<List<ExclusionViewModel>> GetByCycleAsync(int idCycle)
        {
            var resp = await _http.GetAsync($"api/Exclusion/ByCycle/{idCycle}");
            if (!resp.IsSuccessStatusCode) return new();
            return JsonSerializer.Deserialize<List<ExclusionViewModel>>(
                await resp.Content.ReadAsStringAsync(), _json) ?? new();
        }

        public async Task<bool> CreateAsync(ExclusionViewModel exclusion)
        {
            var payload = new
            {
                exclusion.IdCycle,
                exclusion.IdMembre,
                exclusion.Motif,
                exclusion.EstDefinitif,
                exclusion.Notes,
                exclusion.IdLitige
            };
            var body = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return (await _http.PostAsync("api/Exclusion", body)).IsSuccessStatusCode;
        }

        public async Task<bool> ReintegrerAsync(int id)
            => (await _http.PutAsync($"api/Exclusion/{id}/Reintegrer",
                new StringContent("", Encoding.UTF8, "application/json"))).IsSuccessStatusCode;

        public async Task<bool> DeleteAsync(int id)
            => (await _http.DeleteAsync($"api/Exclusion/{id}")).IsSuccessStatusCode;
    }
}
