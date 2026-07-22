using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IChatService
    {
        Task<string> EnvoyerMessageAsync(string message, List<ChatMessageViewModel> historique);
    }
}
