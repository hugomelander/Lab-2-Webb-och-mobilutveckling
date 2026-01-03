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
                },

                new()
                {
                    Title = "Jazz Night",
                    Description = "En kväll med klassisk jazz.",
                    Performances = new List<Performance>
                    {
                        new() { DateTime = DateTime.Today.AddDays(9).AddHours(19), Location = "Stora Scenen" },
                        new() { DateTime = DateTime.Today.AddDays(10).AddHours(20), Location = "Stora Scenen" }
                    }
                },
                                
                new()
                {
                    Title = "Blues Night",
                    Description = "En kväll med klassisk blues.",
                    Performances = new List<Performance>
                    {
                        new() { DateTime = DateTime.Today.AddDays(11).AddHours(19), Location = "Stora Scenen" },
                        new() { DateTime = DateTime.Today.AddDays(12).AddHours(20), Location = "Stora Scenen" }
                    }
                }
            };

            db.Concerts.AddRange(concerts);
            db.SaveChanges();
            Console.WriteLine(">>> Seed klar");
        }
    }
}
