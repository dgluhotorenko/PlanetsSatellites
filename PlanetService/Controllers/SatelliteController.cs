using Microsoft.AspNetCore.Mvc;

namespace PlanetService.Controllers;

[Route("api/p/[controller]")]
[ApiController]
public class SatelliteController : ControllerBase
{
    public SatelliteController()
    {
        
    }
    
    [HttpPost]
    public IActionResult TestInbound()
    {
        Console.WriteLine("Inbound POST PlanetService");

        return Ok("TestInbound from SatelliteController");
    }
}