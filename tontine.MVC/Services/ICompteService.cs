using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface ICompteService
    {
        Task<List<CompteUtilisateurViewModel>> GetAllAsync();
        Task<CompteUtilisateurViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CompteUtilisateurViewModel vm);
        Task<bool> UpdateAsync(CompteUtilisateurViewModel vm);
        Task<bool> DeleteAsync(int id);
        Task<UserSessionDto?> LoginAsync(string nom, string motDePasse);
        Task<(bool Success, string? Erreur)> ChangePasswordAsync(int id, string ancienMdp, string nouveauMdp);
    }
}
