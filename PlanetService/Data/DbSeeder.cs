using Microsoft.EntityFrameworkCore;
using PlanetService.Models;

namespace PlanetService.Data;

public static class DbSeeder
{
    public static void Seed(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        if (env.IsProduction())
        {
            logger.LogInformation("Applying database migrations...");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to apply database migrations");
                throw;
            }
        }
        else
        {
            context.Database.EnsureCreated();
        }

        if (context.Planets.Any())
        {
            logger.LogInformation("Database already seeded — skipping");
            return;
        }

        logger.LogInformation("Seeding database with initial planet data...");

        context.Planets.AddRange(
            new Planet { Name = "Mercury", Mass = 0.055, Radius = 2439.7 },
            new Planet { Name = "Venus", Mass = 0.815, Radius = 6051.8 },
            new Planet { Name = "Earth", Mass = 1.0, Radius = 6371.0 },
            new Planet { Name = "Mars", Mass = 0.107, Radius = 3389.5 },
            new Planet { Name = "Jupiter", Mass = 317.8, Radius = 69911.0 }
        );

        context.SaveChanges();
        logger.LogInformation("Database seeded successfully");
    }
}
