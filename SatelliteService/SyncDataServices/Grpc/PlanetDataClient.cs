using Grpc.Net.Client;
using PlanetService;
using SatelliteService.Mappers;
using SatelliteService.Models;
using SatelliteService.SyncDataServices.Grpc.Abstract;

namespace SatelliteService.SyncDataServices.Grpc;

public class PlanetDataClient(IConfiguration configuration) : IPlanetDataClient
{
    public IEnumerable<Planet> GetAll()
    {
        var address = configuration["GrpcPlanet"];

        Console.WriteLine($"==> Calling Planet gRPC Service {address}");

        var channel = GrpcChannel.ForAddress(address!);
        var client = new GrpcPlanet.GrpcPlanetClient(channel);
        var request = new GetAllRequest();

        try
        {
            var reply = client.GetAll(request);
            return reply.Planets.ToModels();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"==> Could not call Planet gRPC Service {ex.Message}");
            throw;
        }
    }
}