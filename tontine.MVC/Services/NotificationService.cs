using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class NotificationService : INotificationService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public NotificationService(HttpClient http) => _http = http;

        public async Task<List<NotificationViewModel>> GetAllAsync()
        {
            var r = await _http.GetAsync("api/Notification");
            if (!r.IsSuccessStatusCode) return new();
            var json = await r.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<NotificationViewModel>>(json, _json) ?? new();
        }

        public async Task<int> GetNonLuesCountAsync()
        {
            var r = await _http.GetAsync("api/Notification/nonlues");
            if (!r.IsSuccessStatusCode) return 0;
            var json = await r.Content.ReadAsStringAsync();
            var obj  = JsonSerializer.Deserialize<JsonElement>(json, _json);
            return obj.TryGetProperty("count", out var p) ? p.GetInt32() : 0;
        }

        public async Task<bool> MarquerLueAsync(int id)
        {
            var r = await _http.PutAsync($"api/Notification/{id}/lire", null);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> MarquerToutesLuesAsync()
        {
            var r = await _http.PutAsync("api/Notification/lire-tout", null);
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var r = await _http.DeleteAsync($"api/Notification/{id}");
            return r.IsSuccessStatusCode;
        }

        public async Task<bool> SupprimerLuesAsync()
        {
            var r = await _http.DeleteAsync("api/Notification/supprimer-lues");
            return r.IsSuccessStatusCode;
        }
    }
}
