using System;
using System.Collections.Generic;
using System.Text;

namespace ConcertBooking.Mobile.Models
{
    public class PerformanceDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = "";
        public int ConcertId { get; set; }
        public int BookingCount { get; set; }
    }
}
