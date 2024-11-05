using Microsoft.AspNetCore.Mvc;
using PlanetService.Data.Abstract;
using PlanetService.DTOs;
using PlanetService.Mappers;
using PlanetService.SyncDataServices.Http;

namespace PlanetService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlanetController(IPlanetRepository planetRepository, IPlanetDataClient planetDataClient) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlanetReadDto>> GetAll() => Ok(planetRepository.GetAll().ToReadDtos());

    [HttpGet("{id:int}")]
    public ActionResult<PlanetReadDto> GetById(int id)
    {
        ActionResult result = NotFound();

        var planet = planetRepository.GetById(id);
        if (planet != null)
        {
            result = Ok(planet.ToReadDto());
        }

        return result;
    }


    [HttpPost]
    public async Task<ActionResult<PlanetReadDto>> CreateAsync(PlanetCreateDto planetCreateDto)
    {
        var model = planetCreateDto.ToModel();
        planetRepository.Create(model);
        planetRepository.SaveChanges();

        var planetReadDto = model.ToReadDto();

        try
        {
            await planetDataClient.SendPlanetDataAsync(planetReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"==> Could not send data synchronously to SatelliteService: {e.Message}");
        }

        return CreatedAtAction(nameof(GetById), new { id = planetReadDto.Id }, planetReadDto);
    }
}