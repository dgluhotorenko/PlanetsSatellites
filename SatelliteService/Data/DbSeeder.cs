using SatelliteService.Models;

namespace SatelliteService.Data;

public static class DbSeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
        context?.Database.EnsureCreated();

        if (context != null && !context.Satellites.Any())
        {
            Console.WriteLine("==> Seeding database...");

            context.Satellites.AddRange(
                new Satellite { Id = 1, Name = "Space Station", Type = "Weather", Orbit = "Medium Earth" },
                new Satellite { Id = 2, Name = "Satcom", Type = "Communications", Orbit = "Geostationary" },
                new Satellite { Id = 3, Name = "Echo ", Type = "Observation", Orbit = "Geosynchronous" },
                new Satellite { Id = 4, Name = "Explorer ", Type = "Navigation", Orbit = "Sun-synchronous" }
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("Database already seeded.");
        }
    }
}