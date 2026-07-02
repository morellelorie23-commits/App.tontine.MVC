using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface ILitigeService
    {
        Task<List<LitigeViewModel>> GetByCycleAsync(int idCycle);
        Task<LitigeViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(LitigeViewModel litige);
        Task<bool> ResoudreAsync(int id, string resolution);
        Task<bool> ClasserAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
