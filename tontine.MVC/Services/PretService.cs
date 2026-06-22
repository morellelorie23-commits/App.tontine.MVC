using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IPretService
    {
        Task<List<PretViewModel>> GetAllAsync();
        Task<PretViewModel?> GetByIdAsync(int id);
        Task<bool> CreateAsync(PretViewModel pret);
        Task<(bool Success, string? Error)> CreateWithErrorAsync(PretViewModel pret);
        Task<bool> UpdateAsync(int id, PretViewModel pret);
        Task<bool> DeleteAsync(int id);
        Task<List<PretViewModel>> GetByMembreAsync(int idMembre);
        Task<bool> ApprouverAsync(int id);
        Task<bool> RemboursaAsync(int id);
    }

    public class PretService : IPretService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public PretService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<PretViewModel>> GetAllAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Pret");
                if (!response.IsSuccessStatusCode) return new();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PretViewModel>>(json, _jsonOptions) ?? new();
            }
            catch { return new(); }
        }

        public async Task<PretViewModel?> GetByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Pret/{id}");
                if (!response.IsSuccessStatusCode) return null;
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<PretViewModel>(json, _jsonOptions);
            }
            catch { return null; }
        }

        public async Task<(bool Success, string? Error)> CreateWithErrorAsync(PretViewModel pret)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Pret", pret);
                if (response.IsSuccessStatusCode) return (true, null);
                var msg = await response.Content.ReadAsStringAsync();
                return (false, msg.Trim('"'));
            }
            catch { return (false, "Erreur de connexion au serveur."); }
        }

        public async Task<bool> CreateAsync(PretViewModel pret)
        {
            var (ok, _) = await CreateWithErrorAsync(pret);
            return ok;
        }

        public async Task<bool> UpdateAsync(int id, PretViewModel pret)
        {
            try { return (await _httpClient.PutAsJsonAsync($"api/Pret/{id}", pret)).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try { return (await _httpClient.DeleteAsync($"api/Pret/{id}")).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<List<PretViewModel>> GetByMembreAsync(int idMembre)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Pret/membre/{idMembre}");
                if (!response.IsSuccessStatusCode) return new();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PretViewModel>>(json, _jsonOptions) ?? new();
            }
            catch { return new(); }
        }

        public async Task<bool> ApprouverAsync(int id)
        {
            try { return (await _httpClient.PostAsync($"api/Pret/{id}/approuver", null)).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<bool> RemboursaAsync(int id)
        {
            try { return (await _httpClient.PostAsync($"api/Pret/{id}/rembourser", null)).IsSuccessStatusCode; }
            catch { return false; }
        }
    }
}
