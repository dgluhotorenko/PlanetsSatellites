using Microsoft.AspNetCore.Mvc;
using SatelliteService.Data.Abstract;
using SatelliteService.DTOs;
using SatelliteService.Mappers;

namespace SatelliteService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SatellitesController(ISatelliteRepository repository) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<SatelliteReadDto>> GetAll() => Ok(repository.GetAll().ToReadDtos());
}