using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class LitigeService : ILitigeService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public LitigeService(HttpClient http) => _http = http;

        public async Task<List<LitigeViewModel>> GetByCycleAsync(int idCycle)
        {
            var resp = await _http.GetAsync($"api/Litige/ByCycle/{idCycle}");
            if (!resp.IsSuccessStatusCode) return new();
            return JsonSerializer.Deserialize<List<LitigeViewModel>>(
                await resp.Content.ReadAsStringAsync(), _json) ?? new();
        }

        public async Task<LitigeViewModel?> GetByIdAsync(int id)
        {
            var resp = await _http.GetAsync($"api/Litige/{id}");
            if (!resp.IsSuccessStatusCode) return null;
            return JsonSerializer.Deserialize<LitigeViewModel>(
                await resp.Content.ReadAsStringAsync(), _json);
        }

        public async Task<bool> CreateAsync(LitigeViewModel litige)
        {
            var payload = new
            {
                litige.IdCycle,
                litige.IdMembreAccuse,
                litige.IdMembreDeclarant,
                litige.Nature,
                litige.Description,
                litige.Gravite
            };
            var body = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            return (await _http.PostAsync("api/Litige", body)).IsSuccessStatusCode;
        }

        public async Task<bool> ResoudreAsync(int id, string resolution)
        {
            var body = new StringContent(
                JsonSerializer.Serialize(new { Resolution = resolution }),
                Encoding.UTF8, "application/json");
            return (await _http.PutAsync($"api/Litige/{id}/Resoudre", body)).IsSuccessStatusCode;
        }

        public async Task<bool> ClasserAsync(int id)
        {
            return (await _http.PutAsync($"api/Litige/{id}/Classer",
                new StringContent("", Encoding.UTF8, "application/json"))).IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
            => (await _http.DeleteAsync($"api/Litige/{id}")).IsSuccessStatusCode;
    }
}
