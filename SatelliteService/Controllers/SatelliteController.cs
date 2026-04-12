using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.Mappers;

namespace SatelliteService.Controllers;

[Route("api/s/planets/{planetId:int}/[controller]")]
[ApiController]
[Authorize]
public class SatelliteController(ISatelliteRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<SatelliteReadDto>> GetSatellitesForPlanet(int planetId)
    {
        if (!repository.IsPlanetExists(planetId))
            return NotFound();

        return Ok(repository.GetSatellitesByPlanetId(planetId).ToReadDtos());
    }

    [HttpGet("{satelliteId:int}", Name = "GetSatelliteForPlanet")]
    public ActionResult<SatelliteReadDto> GetSatelliteForPlanet(int planetId, int satelliteId)
    {
        if (!repository.IsPlanetExists(planetId))
            return NotFound();

        var satellite = repository.GetSatellite(planetId, satelliteId);
        if (satellite is null)
            return NotFound();

        return Ok(satellite.ToReadDto());
    }

    [HttpPost]
    public ActionResult<SatelliteReadDto> CreateSatelliteForPlanet(int planetId, SatelliteCreateDto dto)
    {
        if (!repository.IsPlanetExists(planetId))
            return NotFound();

        var satellite = dto.ToModel();
        repository.CreateSatellite(planetId, satellite);
        repository.SaveChanges();

        return CreatedAtRoute(
            nameof(GetSatelliteForPlanet),
            new { planetId, satelliteId = satellite.Id },
            satellite.ToReadDto());
    }
}
