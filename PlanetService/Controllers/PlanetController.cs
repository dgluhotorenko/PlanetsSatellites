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
public class PlanetController(IPlanetRepository planetRepository,
    IHttpDataClient httpDataClient,
    IMessageBusDataClient messageBusDataClient) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public ActionResult<IEnumerable<PlanetReadDto>> GetAll() => Ok(planetRepository.GetAll().ToReadDtos());

    [HttpGet("{id:int}")]
    [Authorize]
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
    [Authorize]
    public async Task<ActionResult<PlanetReadDto>> CreateAsync(PlanetCreateDto planetCreateDto)
    {
        var model = planetCreateDto.ToModel();
        planetRepository.Create(model);
        planetRepository.SaveChanges();

        var planetReadDto = model.ToReadDto();

        // Send sync message by http
        try
        {
            await httpDataClient.SendPlanetDataAsync(planetReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"==> Could not send data synchronously: {e.Message}");
        }

        // Send async message by RabbitMQ
        try
        {
            var planetPublishedDto = planetReadDto.ToPublishedDto();
            planetPublishedDto.Event = "Planet_Published";
            await messageBusDataClient.InitializeAsync();
            await messageBusDataClient.PublishNewPlanetAsync(planetPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"==> Could not send data asynchronously: {e.Message}");
        }
        finally
        {
            await messageBusDataClient.DisposeAsync();
        }

        return CreatedAtAction(nameof(GetById), new { id = planetReadDto.Id }, planetReadDto);
    }
}