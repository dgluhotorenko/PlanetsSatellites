using Microsoft.AspNetCore.Mvc;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.Mappers;

namespace SatelliteService.Controllers;

[Route("api/p/planets/{planetId}/[controller]")]
[ApiController]
public class SatelliteController(ISatelliteRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<SatelliteReadDto>> GetSatellitesForPlanet(int planetId)
    {
        Console.WriteLine($"==> GET Satellites from SatelliteService for planetId: {planetId}");

        return !repository.IsPlanetExists(planetId)
            ? NotFound()
            : Ok(repository.GetSatellitesByPlanetId(planetId).ToReadDtos());
    }

    [HttpGet("{satelliteId}", Name = "GetSatelliteForPlanet")]
    public ActionResult<SatelliteReadDto> GetSatelliteForPlanet(int planetId, int satelliteId)
    {
        ActionResult result;

        Console.WriteLine(
            $"==> GET Satellite from SatelliteService for planetId: {planetId} and satelliteId: {satelliteId}");

        if (!repository.IsPlanetExists(planetId))
        {
            result = NotFound();
        }
        else
        {
            var satellite = repository.GetSatellite(planetId, satelliteId);

            result = satellite == null
                ? NotFound()
                : Ok(satellite.ToReadDto());
        }

        return result;
    }

    [HttpPost]
    public ActionResult<SatelliteReadDto> CreateSatelliteForPlanet(int planetId, SatelliteCreateDto satelliteCreateDto)
    {
        ActionResult<SatelliteReadDto> result;

        Console.WriteLine($"==> POST Satellite from SatelliteService for planetId: {planetId}");

        if (!repository.IsPlanetExists(planetId))
        {
            result = NotFound();
        }
        else
        {
            var satellite = satelliteCreateDto.ToModel();
            repository.CreateSatellite(planetId, satellite);
            repository.SaveChanges();

            result = CreatedAtRoute(nameof(GetSatelliteForPlanet), new { planetId, satelliteId = satellite.Id }, satellite.ToReadDto());
        }

        return result;
    }
}