using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface ICycleTontineService
    {
        Task<List<CycleTontineViewModel>> GetAllAsync();
        Task<CycleTontineViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CycleTontineViewModel cycleTontine);
        Task<(bool Success, string? Error)> CreateWithErrorAsync(CycleTontineViewModel cycleTontine);
        Task<bool> UpdateAsync(int id, CycleTontineViewModel cycleTontine);
        Task<bool> DeleteAsync(int id);
        Task<(int Crees, int Membres, string? Erreur)> RepartirAsync(int id);
    }
}
