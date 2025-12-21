using ConcertBooking.API.Data;
using ConcertBooking.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ConcertBooking.API.Repositories;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly ApplicationDbContext _db;

    public IRepository<Concert> Concerts { get; }
    public IRepository<Performance> Performances { get; }
    public IRepository<Booking> Bookings { get; }
    public async Task<List<Concert>> GetConcertsWithBookingsAsync()
    {
        return await _db.Concerts
            .Include(c => c.Performances)
                .ThenInclude(p => p.Bookings)
            .ToListAsync();
    }
    public async Task<List<Performance>> GetPerformancesWithBookingsAsync(int concertId)
    {
        return await _db.Performances
            .Where(p => p.ConcertId == concertId)
            .Include(p => p.Bookings)
            .ToListAsync();
    }


    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;

        Concerts = new Repository<Concert>(_db);
        Performances = new Repository<Performance>(_db);
        Bookings = new Repository<Booking>(_db);
    }

    public async Task SaveAsync()
        => await _db.SaveChangesAsync();

    public void Dispose()
        => _db.Dispose();
}
