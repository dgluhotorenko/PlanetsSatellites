using Grpc.Core;
using PlanetService.Data.Abstract;
using PlanetService.Mappers;

namespace PlanetService.SyncDataServices.Grpc;

public class GrpcPlanetService(
    IPlanetRepository repository,
    ILogger<GrpcPlanetService> logger) : GrpcPlanet.GrpcPlanetBase
{
    public override Task<PlanetResponse> GetAll(GetAllRequest request, ServerCallContext context)
    {
        var response = new PlanetResponse();
        var planets = repository.GetAll();

        foreach (var planet in planets)
        {
            response.Planets.Add(planet.ToGrpcModel());
        }

        logger.LogInformation("Served {Count} planets via gRPC", response.Planets.Count);
        return Task.FromResult(response);
    }
}
