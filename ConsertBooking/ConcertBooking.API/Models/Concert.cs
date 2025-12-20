namespace ConcertBooking.API.Models
{
    public class Concert
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";

        public ICollection<Performance> Performances { get; set; } = new List<Performance>();
    }
}
