using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IMembrePosteCycleService
    {
        Task<List<MembrePosteCycleViewModel>> GetAllAsync();
        Task<MembrePosteCycleViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MembrePosteCycleViewModel membrePosteCycle);
        Task<bool> UpdateAsync(int id, MembrePosteCycleViewModel membrePosteCycle);
        Task<bool> DeleteAsync(int id);
    }
}
