using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IGrandLivreService
    {
        Task<GrandLivreViewModel?> GetGrandLivreAsync(int idTontine, string? periode = null);
        Task<JournalViewModel?> GetJournalAsync(string codeJournal, string? periode = null);
        Task<BalanceViewModel?> GetBalanceAsync(string? periode = null);
        Task<RapprochementViewModel?> GetRapprochementAsync(int idTontine, decimal? soldeReelBanque = null);

        // Journée
        Task<JourneeComptableViewModel?> GetJourneeCouranteAsync();
        Task<JourneeComptableViewModel?> OuvrirJourneeAsync(DateOnly date, string ouvertPar);
        Task<bool> FermerJourneeAsync(int id);

        // Relevé caisse
        Task<ReleveCaisseViewModel?> GetReleveCaisseAsync(string? compte, string? dateDebut, string? dateFin);

        // Balance générale & client
        Task<BalanceDetailViewModel?> GetBalanceGeneraleAsync(string? dateDebut, string? dateFin);
        Task<BalanceDetailViewModel?> GetBalanceClientAsync(string? dateDebut, string? dateFin, string? prefixeCompte);

        // Opération manuelle & transfert caisse (retourne IdEcriture)
        Task<int?> CreerEcritureManuelleAsync(object payload);
        Task<int?> TransfertCaisseAsync(object payload);
    }
}
