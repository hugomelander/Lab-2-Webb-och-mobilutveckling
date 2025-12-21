using System.ComponentModel.DataAnnotations;

namespace ConcertBooking.API.Models
{
    public class CreateBookingDto
    {
        [Required]
        public int PerformanceId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; } = "";

        [Required, EmailAddress, StringLength(320)]
        public string Email { get; set; } = "";
    }
}
