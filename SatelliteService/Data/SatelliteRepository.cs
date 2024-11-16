using SatelliteService.Data.Abstract;
using SatelliteService.Models;

namespace SatelliteService.Data;

public class SatelliteRepository(AppDbContext context) : ISatelliteRepository
{
    public bool SaveChanges() => context.SaveChanges() >= 0;

    public IEnumerable<Planet> GetAllPlanets() => context.Planets
        .OrderBy(p => p.Name)
        .ToList();

    public void CreatePlanet(Planet planet)
    {
        ArgumentNullException.ThrowIfNull(planet);

        context.Planets.Add(planet);
    }

    public bool IsPlanetExists(int planetId) => context.Planets.Any(p => p.Id == planetId);

    public bool IsExternalPlanetExists(int externalPlanetId) => context.Planets.Any(p => p.ExternalId == externalPlanetId);


    public IEnumerable<Satellite> GetSatellitesByPlanetId(int planetId) => context.Satellites
            .Where(s => s.PlanetId == planetId)
            .OrderBy(s => s.Name)
            .ToList();

    public Satellite GetSatellite(int planetId, int satelliteId) => context.Satellites
            .FirstOrDefault(s => s.PlanetId == planetId && s.Id == satelliteId)!;

    public void CreateSatellite(int planetId, Satellite satellite)
    {
        ArgumentNullException.ThrowIfNull(satellite);

        satellite.PlanetId = planetId;
        context.Satellites.Add(satellite);
    }
}