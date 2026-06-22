using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface ISaisieSeanceService
    {
        Task<List<MembreSelectDto>> GetMembresAsync(int idTontine, int idCycle);
        Task<List<ReunionSelectDto>> GetReunionsAsync(int idTontine, int idCycle);
        Task<(decimal SoldeCaisse, List<LigneSeanceViewModel> Lignes, List<HistoriqueBeneficiaireDto> Dejabeneficiaires)>
            GetDataAsync(int idTontine, int idReunion, int idCycle);
        Task<(bool Success, string Message)> EnregistrerAsync(SaisieSeanceSaveDto dto);
    }
}
