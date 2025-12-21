using ConcertBooking.API.Models;

namespace ConcertBooking.API.Data
{
    public static class Seed
    {
        public static void SeedData(ApplicationDbContext db)
        {
            Console.WriteLine(">>> SeedData() körs");

            if (db.Concerts.Any())
            {
                Console.WriteLine(">>> Seed hoppades över: Concerts finns redan");
                return;
            }

            Console.WriteLine(">>> Seedar: lägger in konserter...");

            var concerts = new List<Concert>
            {
                new()
                {
                    Title = "Rock Night",
                    Description = "En kväll med klassisk rock.",
                    Performances = new List<Performance>
                    {
                        new() { DateTime = DateTime.Today.AddDays(7).AddHours(19), Location = "Stora Scenen" },
                        new() { DateTime = DateTime.Today.AddDays(8).AddHours(20), Location = "Stora Scenen" }
                    }
                }
            };

            db.Concerts.AddRange(concerts);
            db.SaveChanges();
            Console.WriteLine(">>> Seed klar");
        }
    }
}
