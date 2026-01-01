using System;
using System.Collections.Generic;
using System.Text;
using ConcertBooking.Mobile.Models;

namespace ConcertBooking.Mobile.Models
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int PerformanceId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
