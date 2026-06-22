using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IStatistiqueService
    {
        Task<StatistiqueViewModel> GetStatsAsync();
        Task<object?> GetStatsTontineAsync(int id);
    }
}
