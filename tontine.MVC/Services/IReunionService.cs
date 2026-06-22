using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IReunionService
    {
        Task<List<ReunionViewModel>> GetAllAsync();
        Task<ReunionViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(ReunionViewModel vm);
        Task<bool> UpdateAsync(int id, ReunionViewModel vm);
        Task<bool> DeleteAsync(int id);
    }
}
