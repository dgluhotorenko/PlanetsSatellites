using PlanetService.DTOs;

namespace PlanetService.SyncDataServices.Http;

public interface IPlanetDataClient
{
    Task SendPlanetDataAsync(PlanetReadDto planet);
}