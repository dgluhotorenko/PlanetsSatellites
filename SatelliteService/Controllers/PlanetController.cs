using Microsoft.AspNetCore.Mvc;

namespace SatelliteService.Controllers;

[Route("api/p/[controller]")]
[ApiController]
public class PlanetController : ControllerBase
{
    [HttpPost]
    public IActionResult TestInbound()
    {
        Console.WriteLine("==> Inbound POST SatelliteService");

        return Ok("TestInbound from PlanetController");
    }
}