using Grpc.Net.Client;
using PlanetService;
using SatelliteService.Mappers;
using SatelliteService.Models;
using SatelliteService.SyncDataServices.Grpc.Abstract;

namespace SatelliteService.SyncDataServices.Grpc;

public class PlanetDataClient(
    IConfiguration configuration,
    ILogger<PlanetDataClient> logger) : IPlanetDataClient
{
    public IEnumerable<Planet> GetAll()
    {
        var address = configuration["GrpcPlanet"]
            ?? throw new InvalidOperationException("GrpcPlanet endpoint is not configured");

        logger.LogInformation("Calling PlanetService gRPC endpoint at {Address}", address);

        using var channel = GrpcChannel.ForAddress(address);
        var client = new GrpcPlanet.GrpcPlanetClient(channel);
        var reply = client.GetAll(new GetAllRequest());

        logger.LogInformation("Received {Count} planets from gRPC", reply.Planets.Count);
        return reply.Planets.ToModels();
    }
}
