using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IPresenceService
    {
        Task<List<PresenceViewModel>> GetByReunionAsync(int idReunion);
        Task<bool> BatchSaveAsync(int idReunion, List<PresenceViewModel> presences);
    }
}
