using tontine.MVC.Models;


namespace tontine.MVC.Services
{
    public interface ICotisationService
    {
        Task<List<CotisationViewModel>> GetAllAsync();
        Task<CotisationViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CotisationViewModel cotisation);
        Task<bool> UpdateAsync(int id, CotisationViewModel cotisation);
        Task<bool> DeleteAsync(int id);
        Task<int> AppliquerPenalitesAsync();
        Task<bool> PayerCashAsync(int id);
        Task<List<TourViewModel>> GetToursAsync(int idCycle, int idTontine);
        Task<LivreDeCompteViewModel?> GetLivreDeCompteAsync(int idCycle, int idTontine);
    }
}
