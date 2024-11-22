using SatelliteService.Models;

namespace SatelliteService.SyncDataServices.Grpc.Abstract;

public interface IPlanetDataClient
{
    IEnumerable<Planet> GetAll();
}