using SatelliteService.DTOs;

namespace SatelliteService.SyncDataServices.Http;

public interface IPlanetDataClient
{
    Task SendSatelliteDataAsync(SatelliteReadDto satellite);
}