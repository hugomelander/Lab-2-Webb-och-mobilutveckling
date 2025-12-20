using ConcertBooking.API.Models;
using Microsoft.EntityFrameworkCore;


namespace ConcertBooking.API.Data

{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Concert> Concerts => Set<Concert>();
        public DbSet<Performance> Performances => Set<Performance>();
        public DbSet<Booking> Bookings => Set<Booking>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Concert>()
                .Property(c => c.Title)
                .HasMaxLength(200)
                .IsRequired();

            modelBuilder.Entity<Booking>()
                .Property(b => b.Email)
                .HasMaxLength(320)
                .IsRequired();
        }
    }
}
