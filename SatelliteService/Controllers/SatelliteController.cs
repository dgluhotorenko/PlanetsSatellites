using Microsoft.AspNetCore.Mvc;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.Mappers;
using SatelliteService.SyncDataServices.Http;

namespace SatelliteService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SatelliteController(ISatelliteRepository satelliteRepository, IPlanetDataClient planetDataClient) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<SatelliteReadDto>> GetAll() => Ok(satelliteRepository.GetAll().ToReadDtos());

    [HttpGet("{id:int}")]
    public ActionResult<SatelliteReadDto> GetById(int id)
    {
        ActionResult result = NotFound();

        var satellite = satelliteRepository.GetById(id);
        if (satellite != null)
        {
            result = Ok(satellite.ToReadDto());
        }

        return result;
    }


    [HttpPost]
    public async Task<ActionResult<SatelliteReadDto>> CreateAsync(SatelliteCreateDto satelliteCreateDto)
    {
        var model = satelliteCreateDto.ToModel();
        satelliteRepository.Create(model);
        satelliteRepository.SaveChanges();

        var satelliteReadDto = model.ToReadDto();

        try
        {
            await planetDataClient.SendSatelliteDataAsync(satelliteReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine($"==> Could not send data synchronously to PlanetService: {e.Message}");
        }

        return CreatedAtAction(nameof(GetById), new { id = satelliteReadDto.Id }, satelliteReadDto);
    }
}