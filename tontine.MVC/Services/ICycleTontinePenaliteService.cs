using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface ICycleTontinePenaliteService
    {
        Task<List<CycleTontinePenaliteViewModel>> GetAllAsync();
        Task<CycleTontinePenaliteViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CycleTontinePenaliteViewModel cycleTontinePenalite);
        Task<bool> UpdateAsync(int id, CycleTontinePenaliteViewModel cycleTontinePenalite);
        Task<bool> DeleteAsync(int id);
    }
}
