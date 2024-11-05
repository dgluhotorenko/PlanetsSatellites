using Microsoft.EntityFrameworkCore;
using PlanetService.Models;

namespace PlanetService.Data;

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

        if (context != null && !context.Planets.Any())
        {
            Console.WriteLine("==> Seeding database...");

            context.Planets.AddRange(
                new Planet { Name = "Mercury", Mass = 0.055, Radius = 2439.7 },
                new Planet { Name = "Venus", Mass = 0.815, Radius = 6051.8 },
                new Planet { Name = "Earth", Mass = 1.0, Radius = 6371.0 },
                new Planet { Name = "Mars", Mass = 0.107, Radius = 3389.5 },
                new Planet { Name = "Jupiter", Mass = 317.8, Radius = 69911.0 }
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("Database already seeded.");
        }
    }
}