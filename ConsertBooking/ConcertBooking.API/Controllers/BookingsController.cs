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
        _uow = uow;
    }

    // POST: api/bookings
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // Finns performance?
        var performance = await _uow.Performances.GetByIdAsync(dto.PerformanceId);
        if (performance is null)
            return NotFound($"Performance with id {dto.PerformanceId} not found.");

        // (Valfritt men bra) undvik dublettbokning på samma performance + email
        var existing = await _uow.Bookings.FindAsync(b =>
            b.PerformanceId == dto.PerformanceId &&
            b.Email.ToLower() == dto.Email.ToLower());

        if (existing.Count > 0)
            return Conflict("A booking with this email already exists for this performance.");

        var booking = new Booking
        {
            PerformanceId = dto.PerformanceId,
            Name = dto.Name.Trim(),
            Email = dto.Email.Trim()
        };

        await _uow.Bookings.AddAsync(booking);
        await _uow.SaveAsync();

        // Returnera skapad bokning (minimalt)
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
        if (booking is null)
            return NotFound($"Booking with id {id} not found.");

        _uow.Bookings.Remove(booking);
        await _uow.SaveAsync();

        return NoContent();
    }
}
