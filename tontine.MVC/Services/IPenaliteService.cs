using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IPenaliteService
    {
        Task<List<PenaliteViewModel>> GetAllAsync();
        Task<PenaliteViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(PenaliteViewModel penalite);
        Task<bool> UpdateAsync(int id, PenaliteViewModel penalite);
        Task<bool> DeleteAsync(int id);
    }
}