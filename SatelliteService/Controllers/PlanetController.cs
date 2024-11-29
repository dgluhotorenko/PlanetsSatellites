using Microsoft.AspNetCore.Mvc;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.Mappers;

namespace SatelliteService.Controllers;

[Route("api/s/[controller]")]
[ApiController]
public class PlanetController(ISatelliteRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlanetReadDto>> GetPlanets()
    {
        Console.WriteLine("==> GET Planets from SatelliteService");

        return Ok(repository.GetAllPlanets().ToReadDtos());
    }

    [HttpPost]
    public IActionResult TestInbound()
    {
        Console.WriteLine("==> Inbound POST SatelliteService");

        return Ok("TestInbound from PlanetController");
    }
}