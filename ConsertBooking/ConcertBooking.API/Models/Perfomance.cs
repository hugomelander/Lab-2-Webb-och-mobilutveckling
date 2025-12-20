namespace ConcertBooking.API.Models
{
    public class Performance
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; } = "";

        public int ConcertId { get; set; }
        public Concert? Concert { get; set; }

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
