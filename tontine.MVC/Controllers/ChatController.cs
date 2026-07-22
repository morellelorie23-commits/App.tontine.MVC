using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using tontine.MVC.Models;
using tontine.MVC.Services;

namespace tontine.MVC.Controllers
{
    // Pas de BaseController : le chatbot doit rester accessible au staff ET aux membres,
    // et BaseController redirige automatiquement les membres vers le portail.
    public class ChatController : Controller
    {
        private const string CleSessionHistorique = "chat_historique";
        private const int TailleMaxHistorique = 10;

        private readonly IChatService _chatService;
        public ChatController(IChatService chatService) => _chatService = chatService;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Envoyer([FromBody] EnvoyerMessageRequest req)
        {
            if (HttpContext.Session.GetString("user_id") == null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(req.Message))
                return BadRequest("Message requis.");

            var historique = ChargerHistorique();

            var reponse = await _chatService.EnvoyerMessageAsync(req.Message, historique);

            historique.Add(new ChatMessageViewModel { Role = "user", Contenu = req.Message });
            historique.Add(new ChatMessageViewModel { Role = "assistant", Contenu = reponse });
            if (historique.Count > TailleMaxHistorique)
                historique = historique.Skip(historique.Count - TailleMaxHistorique).ToList();
            SauvegarderHistorique(historique);

            return Ok(new { reponse });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reinitialiser()
        {
            HttpContext.Session.Remove(CleSessionHistorique);
            return Ok();
        }

        private List<ChatMessageViewModel> ChargerHistorique()
        {
            var json = HttpContext.Session.GetString(CleSessionHistorique);
            if (string.IsNullOrEmpty(json)) return new List<ChatMessageViewModel>();
            return JsonSerializer.Deserialize<List<ChatMessageViewModel>>(json) ?? new List<ChatMessageViewModel>();
        }

        private void SauvegarderHistorique(List<ChatMessageViewModel> historique)
        {
            HttpContext.Session.SetString(CleSessionHistorique, JsonSerializer.Serialize(historique));
        }
    }

    public class EnvoyerMessageRequest
    {
        public string Message { get; set; } = "";
    }
}
