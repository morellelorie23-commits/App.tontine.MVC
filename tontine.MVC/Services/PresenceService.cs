using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public PresenceService(HttpClient http) => _http = http;

        public async Task<List<PresenceViewModel>> GetByReunionAsync(int idReunion)
        {
            var response = await _http.GetAsync($"api/PresenceReunion/ByReunion/{idReunion}");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PresenceViewModel>>(json, _json) ?? new();
        }

        public async Task<bool> BatchSaveAsync(int idReunion, List<PresenceViewModel> presences)
        {
            var payload = presences.Select(p => new {
                p.IdPresence,
                p.IdMembre,
                p.StatutPresence,
                p.Remarque
            });
            var content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync($"api/PresenceReunion/BatchSave/{idReunion}", content);
            return response.IsSuccessStatusCode;
        }
    }
}
