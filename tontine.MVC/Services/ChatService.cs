using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class ChatService : IChatService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public ChatService(HttpClient http) => _http = http;

        public async Task<string> EnvoyerMessageAsync(string message, List<ChatMessageViewModel> historique)
        {
            var payload = new
            {
                message,
                historique = historique.Select(h => new { role = h.Role, contenu = h.Contenu })
            };
            var content = new StringContent(
                JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _http.PostAsync("api/Chat", content);
            if (!response.IsSuccessStatusCode)
                return "Désolé, l'assistant est momentanément indisponible.";

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("reponse", out var r)
                ? r.GetString() ?? ""
                : "";
        }
    }
}
