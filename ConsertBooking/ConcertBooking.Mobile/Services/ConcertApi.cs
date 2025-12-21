using System.Text.Json;
using ConcertBooking.Mobile.Models;

namespace ConcertBooking.Mobile.Services;

public class ConcertApi
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public ConcertApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<ConcertDto>> GetConcertsAsync()
    {
        var url = "/api/concerts";
        var response = await _http.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ConcertDto>>(json, _jsonOptions) ?? new List<ConcertDto>();
    }
}
