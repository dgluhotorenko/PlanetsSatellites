using Microsoft.AspNetCore.Mvc;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.Mappers;

namespace SatelliteService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SatelliteController(ISatelliteRepository satelliteRepository) : ControllerBase
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
    public ActionResult<SatelliteReadDto> Create(SatelliteCreateDto satelliteCreateDto)
    {
        var model = satelliteCreateDto.ToModel();
        satelliteRepository.Create(model);
        satelliteRepository.SaveChanges();

        var satelliteReadDto = model.ToReadDto();

        return CreatedAtAction(nameof(GetById), new { id = satelliteReadDto.Id }, satelliteReadDto);
    }
}