using SatelliteService.Data.Abstract;
using SatelliteService.Models;
using SatelliteService.SyncDataServices.Grpc.Abstract;

namespace SatelliteService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using var serviceScope = applicationBuilder.ApplicationServices.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlanetDataClient>();
        var planets = grpcClient?.GetAll();

        SeedData(serviceScope.ServiceProvider.GetService<ISatelliteRepository>()!, planets!);
    }

    private static void SeedData(ISatelliteRepository repository, IEnumerable<Planet> planets)
    {
        Console.WriteLine("==> Seeding new planets...");

        foreach (var planet in planets)
        {
            if (!repository.IsExternalPlanetExists(planet.ExternalId))
            {
                repository.CreatePlanet(planet); 
            }
        }

        repository.SaveChanges();
    }
}