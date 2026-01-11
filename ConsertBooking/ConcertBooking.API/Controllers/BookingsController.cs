using ConcertBooking.API.Models;
using ConcertBooking.API.Repositories;
using Microsoft.AspNetCore.Mvc;
namespace ConcertBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public BookingsController(IUnitOfWork uow)
    {
        _uow = uow; //unitofwork
    }

    // POST: api/bookings
    //Skapa bokning
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // Finns performance?
        var performance = await _uow.Performances.GetByIdAsync(dto.PerformanceId);
        if (performance is null)
            return NotFound($"Performance with id {dto.PerformanceId} not found.");

        // undvik dublettbokning på samma performance + email
        var existing = await _uow.Bookings.FindAsync(b =>
            b.PerformanceId == dto.PerformanceId &&
            b.Email.ToLower() == dto.Email.ToLower());

        //om det finns en sådan bokning, returnera konflikt
        if (existing.Count > 0)
            return Conflict("A booking with this email already exists for this performance.");

        var booking = new Booking
        {
            PerformanceId = dto.PerformanceId,
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim()
        };
        // Spara bokning
        await _uow.Bookings.AddAsync(booking);
        await _uow.SaveAsync();

        // Returnera skapad bokning 
        return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, new
        {
            booking.Id,
            booking.PerformanceId,
            booking.Name,
            booking.Email
        });
    }

    // GET: api/bookings/5  (bara för CreatedAtAction och test)
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var booking = await _uow.Bookings.GetByIdAsync(id);
        if (booking is null)
            return NotFound();

        return Ok(new
        {
            booking.Id,
            booking.PerformanceId,
            booking.Name,
            booking.Email
        });
    }

    // DELETE: api/bookings/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var booking = await _uow.Bookings.GetByIdAsync(id);
        if (booking is null) //om bokningen inte finns
            return NotFound($"Booking with id {id} not found.");

        _uow.Bookings.Remove(booking);
        await _uow.SaveAsync();

        return NoContent();
    }

    // DELETE: api/bookings/by-email?email=test@example.com
    [HttpDelete("by-email")]
    public async Task<IActionResult> DeleteBookingsByEmail([FromQuery] string email, [FromQuery] int performanceId)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");

        if (performanceId <= 0)
            return BadRequest("A valid PerformanceId is required.");

        var normalizedEmail = email.Trim().ToLower();

        // Hämta bara bokningar som matchar BÅDE e-post OCH PerformanceId
        var bookings = await _uow.Bookings.FindAsync(
            b => b.Email.ToLower() == normalizedEmail && b.PerformanceId == performanceId
        );

        if (!bookings.Any())
            return NotFound($"No bookings found for email {email} on this specific performance.");

        foreach (var booking in bookings)
        {
            _uow.Bookings.Remove(booking);
        }

        await _uow.SaveAsync();

        return NoContent();
    }

    //hämta föreställningar för en konsert med bokningar
    [HttpGet("{concertId}/performances")]
    public async Task<IActionResult> GetPerformances(int concertId)
    {
        // Kontrollera om konserten finns
        var performances = await _uow.Performances.FindAsync(
            p => p.ConcertId == concertId,
            p => p.Bookings);

        var dtos = performances.Select(p => new PerformanceDto
        {
            Id = p.Id,
            DateTime = p.DateTime,
            Location = p.Location,
            BookingsCount = p.Bookings.Count,
            Bookings = p.Bookings.Select(b => new BookingDto
            {
                Id = b.Id,
                Name = b.Name,
                Email = b.Email
            }).ToList()
        });

        return Ok(dtos);
    }
    // GET: api/bookings/by-email?email=test@example.com
    [HttpGet("by-email")]
    public async Task<IActionResult> GetBookingsByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is required.");
        // normalisera epost
        var normalizedEmail = email.Trim().ToLower();

        // Inkludera både Performance och tillhörande Concert
        var bookings = await _uow.Bookings.FindAsync(
            b => b.Email.ToLower() == normalizedEmail,
            b => b.Performance,
            b => b.Performance.Concert
        );
        //om inga bokningar finns för den eposten
        if (!bookings.Any())
            return NotFound("No bookings found for this email.");

        var result = bookings.Select(b => new MyBookingDto
        {
            Id = b.Id,
            Name = b.Name,
            Email = b.Email,
            Performance = new BookingPerformanceDto
            {
                Id = b.Performance!.Id,
                DateTime = b.Performance.DateTime,
                Location = b.Performance.Location,
                ConcertId = b.Performance.ConcertId,
                ConcertTitle = b.Performance.Concert?.Title ?? "Okänd konsert"
            }
        }).ToList();

        return Ok(result);
    }
}
