using System.Text;
using System.Text.Json;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class SaisieSeanceService : ISaisieSeanceService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public SaisieSeanceService(HttpClient http) => _http = http;

        public async Task<List<MembreSelectDto>> GetMembresAsync(int idTontine, int idCycle)
        {
            var r = await _http.GetAsync($"api/SaisieSeance/membres?idTontine={idTontine}&idCycle={idCycle}");
            if (!r.IsSuccessStatusCode) return new();
            var json = await r.Content.ReadAsStringAsync();
            var raw = JsonSerializer.Deserialize<List<JsonElement>>(json, _json) ?? new();
            return raw.Select(e => new MembreSelectDto
            {
                IdMembre  = e.GetProperty("idMembre").GetInt32(),
                NomPrenom = e.GetProperty("nomPrenom").GetString() ?? "",
                MtAttendu = e.GetProperty("mtAttendu").GetDecimal()
            }).ToList();
        }

        public async Task<List<ReunionSelectDto>> GetReunionsAsync(int idTontine, int idCycle)
        {
            var r = await _http.GetAsync($"api/SaisieSeance/reunions?idTontine={idTontine}&idCycle={idCycle}");
            if (!r.IsSuccessStatusCode) return new();
            var json = await r.Content.ReadAsStringAsync();
            var raw = JsonSerializer.Deserialize<List<JsonElement>>(json, _json) ?? new();
            return raw.Select(e => new ReunionSelectDto
            {
                IdReunion   = e.GetProperty("idReunion").GetInt32(),
                DateReunion = e.GetProperty("dateReunion").GetString()?[..10] ?? "",
                Objet       = e.TryGetProperty("objet", out var o) ? o.GetString() : null
            }).ToList();
        }

        public async Task<(decimal SoldeCaisse, List<LigneSeanceViewModel> Lignes, List<HistoriqueBeneficiaireDto> Dejabeneficiaires)>
            GetDataAsync(int idTontine, int idReunion, int idCycle)
        {
            var r = await _http.GetAsync($"api/SaisieSeance/data?idTontine={idTontine}&idReunion={idReunion}&idCycle={idCycle}");
            if (!r.IsSuccessStatusCode) return (0, new(), new());

            var root = JsonSerializer.Deserialize<JsonElement>(await r.Content.ReadAsStringAsync(), _json);

            var soldeCaisse = root.GetProperty("soldeCaisse").GetDecimal();

            var lignes = root.GetProperty("lignes").EnumerateArray().Select(e => new LigneSeanceViewModel
            {
                IdMembre        = e.GetProperty("idMembre").GetInt32(),
                NomPrenom       = e.GetProperty("nomPrenom").GetString() ?? "",
                MtAttendu       = e.GetProperty("mtAttendu").GetDecimal(),
                MtCotise        = e.GetProperty("mtCotise").GetDecimal(),
                Penalite        = e.GetProperty("penalite").GetDecimal(),
                MtEnchere       = e.GetProperty("mtEnchere").GetDecimal(),
                IsGagnantEnchere = e.GetProperty("isGagnantEnchere").GetBoolean()
            }).ToList();

            var dejabeneficiaires = root.GetProperty("dejabeneficiaires").EnumerateArray().Select(e => new HistoriqueBeneficiaireDto
            {
                NomPrenom     = e.GetProperty("nomPrenom").GetString() ?? "",
                DateVersement = e.GetProperty("dateVersement").GetString() ?? "",
                MontantNet    = e.GetProperty("montantNet").GetDecimal()
            }).ToList();

            return (soldeCaisse, lignes, dejabeneficiaires);
        }

        public async Task<(bool Success, string Message)> EnregistrerAsync(SaisieSeanceSaveDto dto)
        {
            var payload = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            var r = await _http.PostAsync("api/SaisieSeance/enregistrer", payload);
            var body = await r.Content.ReadAsStringAsync();
            if (r.IsSuccessStatusCode)
            {
                using var doc = JsonDocument.Parse(body);
                return (true, doc.RootElement.GetProperty("message").GetString() ?? "OK");
            }
            return (false, body);
        }
    }
}
