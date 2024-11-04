using Microsoft.EntityFrameworkCore;
using SatelliteService.Models;

namespace SatelliteService.Data;

public static class DbSeeder
{
    public static void Seed(IApplicationBuilder app, bool isProduction)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<AppDbContext>();
        context?.Database.EnsureCreated();

        if (isProduction)
        {
            Console.WriteLine("==> Production environment, applying migrations...");
            try
            {
                context?.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine($"==> Error applying migrations: {e.Message}");
            }
        }

        if (context != null && !context.Satellites.Any())
        {
            Console.WriteLine("==> Seeding database...");

            context.Satellites.AddRange(
                new Satellite { Name = "Space Station", Type = "Weather", Orbit = "Medium Earth" },
                new Satellite { Name = "Satcom", Type = "Communications", Orbit = "Geostationary" },
                new Satellite { Name = "Echo ", Type = "Observation", Orbit = "Geosynchronous" },
                new Satellite { Name = "Explorer ", Type = "Navigation", Orbit = "Sun-synchronous" }
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("Database already seeded.");
        }
    }
}