using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IMembreService
    {
        Task<List<MembreViewModel>> GetAllAsync();
        Task<MembreViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MembreViewModel membre);
        Task<bool> UpdateAsync(int id, MembreViewModel membre);
        Task<bool> DeleteAsync(int id);
        Task<ReleveMembreViewModel?> GetReleveAsync(int id);
        Task<ScoreMembreViewModel?> GetScoreAsync(int id);
        Task<List<ScoreMembreViewModel>> GetAllScoresAsync();
    }
}
