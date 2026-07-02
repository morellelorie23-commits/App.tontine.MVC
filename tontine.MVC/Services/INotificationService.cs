using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface INotificationService
    {
        Task<List<NotificationViewModel>> GetAllAsync();
        Task<int> GetNonLuesCountAsync();
        Task<bool> MarquerLueAsync(int id);
        Task<bool> MarquerToutesLuesAsync();
        Task<bool> DeleteAsync(int id);
        Task<bool> SupprimerLuesAsync();
    }
}
