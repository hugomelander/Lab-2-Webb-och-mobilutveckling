using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ConcertBooking.Mobile.Models; 

public class PerformanceDto
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public string Location { get; set; } = "";
    public int ConcertId { get; set; }

    public List<BookingDto> Bookings { get; set; } = new List<BookingDto>();
}