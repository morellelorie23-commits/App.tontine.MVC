using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public interface IGarantService
    {
        Task<List<GarantViewModel>> GetAllAsync();
        Task<GarantViewModel?> GetByIdAsync(int id);
        Task<GarantViewModel?> GetByMembreAsync(int idMembre);
        Task<EligibiliteViewModel?> CheckEligibiliteAsync(int idMembre);
        Task<bool> CreateAsync(GarantViewModel garant);
        Task<bool> UpdateAsync(int id, GarantViewModel garant);
        Task<bool> DeleteAsync(int id);
    }

    public class GarantService : IGarantService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _opts = new() { PropertyNameCaseInsensitive = true };

        public GarantService(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<List<GarantViewModel>> GetAllAsync()
        {
            try
            {
                var r = await _httpClient.GetAsync("api/Garant");
                if (!r.IsSuccessStatusCode) return new();
                return JsonSerializer.Deserialize<List<GarantViewModel>>(await r.Content.ReadAsStringAsync(), _opts) ?? new();
            }
            catch { return new(); }
        }

        public async Task<GarantViewModel?> GetByIdAsync(int id)
        {
            try
            {
                var r = await _httpClient.GetAsync($"api/Garant/{id}");
                if (!r.IsSuccessStatusCode) return null;
                return JsonSerializer.Deserialize<GarantViewModel>(await r.Content.ReadAsStringAsync(), _opts);
            }
            catch { return null; }
        }

        public async Task<GarantViewModel?> GetByMembreAsync(int idMembre)
        {
            try
            {
                var r = await _httpClient.GetAsync($"api/Garant/membre/{idMembre}");
                if (!r.IsSuccessStatusCode) return null;
                return JsonSerializer.Deserialize<GarantViewModel>(await r.Content.ReadAsStringAsync(), _opts);
            }
            catch { return null; }
        }

        public async Task<EligibiliteViewModel?> CheckEligibiliteAsync(int idMembre)
        {
            try
            {
                var r = await _httpClient.GetAsync($"api/Garant/membre/{idMembre}/eligible");
                if (!r.IsSuccessStatusCode) return null;
                return JsonSerializer.Deserialize<EligibiliteViewModel>(await r.Content.ReadAsStringAsync(), _opts);
            }
            catch { return null; }
        }

        public async Task<bool> CreateAsync(GarantViewModel garant)
        {
            try { return (await _httpClient.PostAsJsonAsync("api/Garant", garant)).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<bool> UpdateAsync(int id, GarantViewModel garant)
        {
            try { return (await _httpClient.PutAsJsonAsync($"api/Garant/{id}", garant)).IsSuccessStatusCode; }
            catch { return false; }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try { return (await _httpClient.DeleteAsync($"api/Garant/{id}")).IsSuccessStatusCode; }
            catch { return false; }
        }
    }
}
