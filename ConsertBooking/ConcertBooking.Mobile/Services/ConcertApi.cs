using System.Text.Json;
using System.Text;
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

    public async Task<List<ConcertDto>> GetConcertsAsync() // metod för att hämta konserter
    {
        var url = "/api/Concerts";
        var response = await _http.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<ConcertDto>>(json, _jsonOptions) ?? new List<ConcertDto>();
    }
    public async Task<List<PerformanceDto>> GetPerformancesAsync(int concertId) // metod för att hämta föreställningar för en konsert
    {
        var response = await _http.GetAsync($"/api/Concerts/{concertId}/performances");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PerformanceDto>>(json, _jsonOptions) ?? new List<PerformanceDto>();
    }
    public async Task<int> CreateBookingAsync(CreateBookingDto dto) // metod för att skapa bokning
    {
        var json = JsonSerializer.Serialize(dto, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _http.PostAsync("/api/bookings", content);

        if (response.IsSuccessStatusCode)
        {
            // API returnerar booking-id i svaret (CreatedAtAction)
            var body = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            return doc.RootElement.GetProperty("id").GetInt32();
        }

        // Lägg feltext i exception (syns i UI)
        var error = await response.Content.ReadAsStringAsync();
        throw new Exception(error);
    }
    public async Task DeleteBookingAsync(int bookingId) // metod för att ta bort bokning baserat på boknings-ID
    {
        var response = await _http.DeleteAsync($"/api/Bookings/{bookingId}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Kunde inte avboka: {error}");
        }
    }

    public async Task DeleteBookingsByEmailAsync(string email, int performanceId) // metod för att ta bort bokningar baserat på e-post
    {
        var url = $"api/bookings/by-email?email={Uri.EscapeDataString(email)}&performanceId={performanceId}";

        var response = await _http.DeleteAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }

    public async Task<List<MyBookingDto>> GetBookingsByEmailAsync(string email) // metod för att hämta bokningar baserat på e-post
    {
        var response = await _http.GetAsync(
            $"api/bookings/by-email?email={Uri.EscapeDataString(email)}");

        if (!response.IsSuccessStatusCode)
        {
            // Om API returnerar 404 (NotFound), hantera det snyggt
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return new List<MyBookingDto>();

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }

        var json = await response.Content.ReadAsStringAsync();
        
        return JsonSerializer.Deserialize<List<MyBookingDto>>(json, _jsonOptions) ?? new List<MyBookingDto>(); // Returnera tom lista om deserialisering misslyckas
    }
}
