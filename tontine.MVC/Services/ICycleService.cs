using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface ICycleService
    {
        Task<List<CycleViewModel>> GetAllAsync();
        Task<CycleViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CycleViewModel cycle);
        Task<bool> UpdateAsync(int id, CycleViewModel cycle);
        Task<bool> DeleteAsync(int id);
        Task<bool> DemarrerAsync(int id);
        Task<bool> CloturerAsync(int id);
    }
}
