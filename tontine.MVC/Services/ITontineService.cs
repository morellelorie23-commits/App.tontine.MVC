using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface ITontineService
    {
        Task<List<TontineViewModel>> GetAllAsync();
        Task<TontineViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(TontineViewModel tontine);
        Task<bool> UpdateAsync(int id, TontineViewModel tontine);
        Task<bool> DeleteAsync(int id);
    }
}