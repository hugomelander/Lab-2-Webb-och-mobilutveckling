using System;
using System.Collections.Generic;
using System.Text;

namespace ConcertBooking.Mobile.Models
{
    public class MyBookingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public BookingPerformanceDto Performance { get; set; } = new();
    }

    public class BookingPerformanceDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; } = "";
        public int ConcertId { get; set; }
        public string ConcertTitle { get; set; } = "";

    }

}
