namespace ConcertBooking.API.Models; 

public class PerformanceDto
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public int BookingsCount { get; set; }

    public List<BookingDto> Bookings { get; set; } = new();
}

public class BookingDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}