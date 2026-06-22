using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class CompteService : ICompteService
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _opts =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        public CompteService(HttpClient http) => _http = http;

        public async Task<List<CompteUtilisateurViewModel>> GetAllAsync()
        {
            var res = await _http.GetAsync("api/CompteUtilisateur");
            if (!res.IsSuccessStatusCode) return new List<CompteUtilisateurViewModel>();
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<CompteUtilisateurViewModel>>(json, _opts)
                   ?? new List<CompteUtilisateurViewModel>();
        }

        public async Task<CompteUtilisateurViewModel?> GetByIdAsync(int id)
        {
            var res = await _http.GetAsync($"api/CompteUtilisateur/{id}");
            if (!res.IsSuccessStatusCode) return null;
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CompteUtilisateurViewModel>(json, _opts);
        }

        public async Task<bool> CreateAsync(CompteUtilisateurViewModel vm)
        {
            var json = JsonSerializer.Serialize(vm);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _http.PostAsync("api/CompteUtilisateur", content);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(CompteUtilisateurViewModel vm)
        {
            var json = JsonSerializer.Serialize(vm);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var res = await _http.PutAsync($"api/CompteUtilisateur/{vm.IdCompte}", content);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/CompteUtilisateur/{id}");
            return res.IsSuccessStatusCode;
        }

        public async Task<UserSessionDto?> LoginAsync(string nom, string motDePasse)
        {
            var payload = new { Nom = nom, MotDePasse = motDePasse };
            var json    = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var res     = await _http.PostAsync("api/Auth/Login", content);
            if (!res.IsSuccessStatusCode) return null;
            var responseJson = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserSessionDto>(responseJson, _opts);
        }

        public async Task<(bool Success, string? Erreur)> ChangePasswordAsync(int id, string ancienMdp, string nouveauMdp)
        {
            try
            {
                var payload = new { AncienMotDePasse = ancienMdp, NouveauMotDePasse = nouveauMdp };
                var res = await _http.PostAsJsonAsync($"api/compteutilisateur/{id}/changepassword", payload);
                if (res.IsSuccessStatusCode) return (true, null);
                var msg = await res.Content.ReadAsStringAsync();
                return (false, msg.Trim('"'));
            }
            catch { return (false, "Une erreur est survenue."); }
        }
    }
}
