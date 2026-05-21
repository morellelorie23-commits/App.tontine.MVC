using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IVersementService
    {
        Task<List<VersementViewModel>> GetAllAsync();
        Task<VersementViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(VersementViewModel versement);
        Task<bool> UpdateAsync(int id, VersementViewModel versement);
        Task<bool> DeleteAsync(int id);
    }
}
