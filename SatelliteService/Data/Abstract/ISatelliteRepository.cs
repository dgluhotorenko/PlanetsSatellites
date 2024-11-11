using SatelliteService.Models;

namespace SatelliteService.Data.Abstract;

public interface ISatelliteRepository
{
    bool SaveChanges();

    IEnumerable<Planet> GetAllPlanets();

    void CreatePlanet(Planet planet);

    bool IsPlanetExists(int planetId);


    IEnumerable<Satellite> GetSatellitesByPlanetId(int planetId);

    Satellite GetSatellite(int planetId, int satelliteId);

    void CreateSatellite(int planetId, Satellite satellite);
}