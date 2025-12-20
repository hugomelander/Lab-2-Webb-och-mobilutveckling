namespace ConcertBooking.API.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int PerformanceId { get; set; }
        public Performance? Performance { get; set; }

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
