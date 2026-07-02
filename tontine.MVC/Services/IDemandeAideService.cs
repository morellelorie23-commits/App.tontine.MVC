using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IDemandeAideService
    {
        Task<List<DemandeAideViewModel>> GetByFondsAsync(int idFonds);
        Task<bool> CreateAsync(DemandeAideViewModel demande);
        Task<bool> ApprouverAsync(int id, decimal montantAccorde, string? notes);
        Task<bool> RejeterAsync(int id, string? notes);
        Task<bool> MarquerPayeAsync(int id);
    }
}
