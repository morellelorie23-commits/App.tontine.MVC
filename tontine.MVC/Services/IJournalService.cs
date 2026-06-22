using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IJournalService
    {
        Task<List<JournalActiviteViewModel>> GetAllAsync();
        Task<List<JournalActiviteViewModel>> GetRecentAsync(int count = 10);
    }
}
