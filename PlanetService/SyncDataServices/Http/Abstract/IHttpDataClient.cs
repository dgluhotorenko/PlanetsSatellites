using PlanetService.DTOs;

namespace PlanetService.SyncDataServices.Http.Abstract;

public interface IHttpDataClient
{
    Task SendPlanetDataAsync(PlanetReadDto planet);
}