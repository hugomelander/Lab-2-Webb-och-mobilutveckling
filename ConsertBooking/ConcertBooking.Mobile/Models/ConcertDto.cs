namespace ConcertBooking.Mobile.Models;

public class ConcertDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int BookingsCount { get; set; }
}
