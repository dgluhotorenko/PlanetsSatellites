using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanetService.AsyncDataServices.Abstract;
using PlanetService.Data.Abstract;
using PlanetService.DTOs;
using PlanetService.Mappers;
using PlanetService.SyncDataServices.Http.Abstract;

namespace PlanetService.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PlanetController(
    IPlanetRepository planetRepository,
    IHttpDataClient httpDataClient,
    IMessageBusDataClient messageBusDataClient,
    ILogger<PlanetController> logger) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlanetReadDto>> GetAll()
    {
        var planets = planetRepository.GetAll().ToReadDtos();
        return Ok(planets);
    }

    [HttpGet("{id:int}")]
    public ActionResult<PlanetReadDto> GetById(int id)
    {
        var planet = planetRepository.GetById(id);
        if (planet is null)
            return NotFound();

        return Ok(planet.ToReadDto());
    }

    [HttpPost]
    public async Task<ActionResult<PlanetReadDto>> CreateAsync(PlanetCreateDto planetCreateDto)
    {
        var model = planetCreateDto.ToModel();
        planetRepository.Create(model);
        planetRepository.SaveChanges();

        var planetReadDto = model.ToReadDto();

        // Notify SatelliteService synchronously via HTTP
        try
        {
            await httpDataClient.SendPlanetDataAsync(planetReadDto);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to send synchronous notification to SatelliteService");
        }

        // Publish event asynchronously via RabbitMQ
        try
        {
            var publishedDto = planetReadDto.ToPublishedDto();
            await messageBusDataClient.PublishNewPlanetAsync(publishedDto);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to publish planet event to message bus");
        }

        return CreatedAtAction(nameof(GetById), new { id = planetReadDto.Id }, planetReadDto);
    }
}
