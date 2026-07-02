using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IExclusionService
    {
        Task<List<ExclusionViewModel>> GetByCycleAsync(int idCycle);
        Task<bool> CreateAsync(ExclusionViewModel exclusion);
        Task<bool> ReintegrerAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
