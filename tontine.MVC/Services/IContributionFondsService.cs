using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IContributionFondsService
    {
        Task<List<ContributionFondsViewModel>> GetByFondsAsync(int idFonds);
        Task<bool> CreateAsync(ContributionFondsViewModel contribution);
        Task<bool> DeleteAsync(int id);
    }
}
