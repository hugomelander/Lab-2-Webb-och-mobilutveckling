using ConcertBooking.API.Models;
using ConcertBooking.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConcertBooking.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConcertsController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public ConcertsController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    // GET: api/concerts
    [HttpGet]
    public async Task<IActionResult> GetConcerts()
    {
        var concerts = await _uow.GetConcertsWithBookingsAsync();

        // Räkna antal bokningar per konsert
        var result = concerts.Select(c => new
        {
            c.Id,
            c.Title,
            c.Description,
            BookingsCount = c.Performances
                .SelectMany(p => p.Bookings)
                .Count()
        });

        return Ok(result);
    }
    // GET: api/concerts/{concertId}/performances
    [HttpGet("{concertId:int}/performances")]
    public async Task<IActionResult> GetPerformances(int concertId)
    {
        // säkerställ att konserten finns
        var concert = await _uow.Concerts.GetByIdAsync(concertId);
        if (concert is null)
            return NotFound($"Concert with id {concertId} not found.");

        var performances = await _uow.GetPerformancesWithBookingsAsync(concertId);

        var result = performances.Select(p => new
        {
            p.Id,
            p.DateTime,
            p.Location,
            p.ConcertId,
            BookingsCount = p.Bookings.Count
        });

        return Ok(result);
    }

}
