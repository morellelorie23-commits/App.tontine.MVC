using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IPosteService
    {
        Task<List<PosteViewModel>> GetAllAsync();
        Task<PosteViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(PosteViewModel poste);
        Task<bool> UpdateAsync(int id, PosteViewModel poste);
        Task<bool> DeleteAsync(int id);
    }
}