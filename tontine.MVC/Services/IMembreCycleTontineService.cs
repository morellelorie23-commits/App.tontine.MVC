using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IMembreCycleTontineService
    {
        Task<List<MembreCycleTontineViewModel>> GetAllAsync();
        Task<MembreCycleTontineViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(MembreCycleTontineViewModel membreCycleTontine);
        Task<bool> UpdateAsync(int id, MembreCycleTontineViewModel membreCycleTontine);
        Task<bool> DeleteAsync(int id);
    }
}
