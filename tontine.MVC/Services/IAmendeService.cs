using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IAmendeService
    {
        Task<List<AmendeViewModel>> GetAllAsync();
        Task<AmendeViewModel?> GetByIdAsync(int id);
        Task<int> GenererAsync(int cycleId);
        Task<bool> MarquerPayeeAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
