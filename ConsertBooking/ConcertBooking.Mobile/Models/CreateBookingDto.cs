namespace ConcertBooking.Mobile.Models;

public class CreateBookingDto
{
    public int PerformanceId { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}
