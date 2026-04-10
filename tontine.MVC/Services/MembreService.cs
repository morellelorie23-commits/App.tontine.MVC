using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using tontine.MVC.Models;

namespace tontine.MVC.Services
{
    public class MembreService : IMembreService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new DateOnlyJsonConverter() }
        };

        public MembreService(HttpClient http) => _http = http;

        public async Task<List<MembreViewModel>> GetAllAsync()
        {
            var response = await _http.GetAsync("api/Membre");
            if (!response.IsSuccessStatusCode) return new List<MembreViewModel>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<MembreViewModel>>(json, _json)
                   ?? new List<MembreViewModel>();
        }

        public async Task<MembreViewModel?> GetByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/Membre/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MembreViewModel>(json, _json);
        }

        public async Task<bool> CreateAsync(MembreViewModel membre)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(membre, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PostAsync("api/Membre", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateAsync(int id, MembreViewModel membre)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(membre, _json),
                Encoding.UTF8,
                "application/json");
            var response = await _http.PutAsync($"api/Membre/{id}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Membre/{id}");
            return response.IsSuccessStatusCode;
        }
    }

    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateOnly.Parse(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString("yyyy-MM-dd"));
    }
}