using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IFondsSolidariteService
    {
        Task<List<FondsSolidariteViewModel>> GetAllAsync();
        Task<FondsSolidariteViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(FondsSolidariteViewModel fonds);
        Task<bool> UpdateAsync(FondsSolidariteViewModel fonds);
        Task<bool> DeleteAsync(int id);
    }
}
