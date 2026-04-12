using SatelliteService.Data.Abstract;
using SatelliteService.SyncDataServices.Grpc.Abstract;

namespace SatelliteService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();
        var grpcClient = scope.ServiceProvider.GetRequiredService<IPlanetDataClient>();
        var repository = scope.ServiceProvider.GetRequiredService<ISatelliteRepository>();

        try
        {
            var planets = grpcClient.GetAll();
            var added = 0;

            foreach (var planet in planets)
            {
                if (!repository.IsExternalPlanetExists(planet.ExternalId))
                {
                    repository.CreatePlanet(planet);
                    added++;
                }
            }

            repository.SaveChanges();
            logger.LogInformation("Seeded {Count} planets from PlanetService via gRPC", added);
        }
        catch (Exception ex)
        {
            // gRPC call to PlanetService may fail if it's not yet available —
            // planets will be synced later via RabbitMQ events
            logger.LogWarning(ex, "Could not fetch planets from PlanetService via gRPC — " +
                                  "planets will be synced via RabbitMQ events as they are created");
        }
    }
}
