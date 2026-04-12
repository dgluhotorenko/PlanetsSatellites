using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.Mappers;

namespace SatelliteService.Controllers;

[Route("api/s/[controller]")]
[ApiController]
[Authorize]
public class PlanetController(ISatelliteRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<PlanetReadDto>> GetPlanets()
    {
        return Ok(repository.GetAllPlanets().ToReadDtos());
    }

    /// <summary>
    /// Test endpoint for synchronous HTTP notifications from PlanetService.
    /// In production, inter-service communication would use only the message bus.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public IActionResult TestInbound()
    {
        return Ok("Inbound POST received by SatelliteService");
    }
}
