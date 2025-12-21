using ConcertBooking.API.Models;

namespace ConcertBooking.API.Repositories;

public interface IUnitOfWork
{
    IRepository<Concert> Concerts { get; }
    IRepository<Performance> Performances { get; }
    IRepository<Booking> Bookings { get; }
    Task<List<Concert>> GetConcertsWithBookingsAsync();
    Task<List<Performance>> GetPerformancesWithBookingsAsync(int concertId);
    


    Task SaveAsync();
}
