using SatelliteService.DTOs;
using SatelliteService.Mappers;
using SatelliteService.Models;

namespace SatelliteService.Tests;

public class SatelliteMapperTests
{
    [Fact]
    public void SatelliteCreateDto_ToModel_MapsProperties()
    {
        var dto = new SatelliteCreateDto { Name = "Moon", Type = "Natural" };

        var model = dto.ToModel();

        Assert.Equal("Moon", model.Name);
        Assert.Equal("Natural", model.Type);
    }

    [Fact]
    public void Satellite_ToReadDto_MapsAllProperties()
    {
        var satellite = new Satellite { Id = 1, Name = "Moon", Type = "Natural", PlanetId = 3 };

        var dto = satellite.ToReadDto();

        Assert.Equal(1, dto.Id);
        Assert.Equal("Moon", dto.Name);
        Assert.Equal("Natural", dto.Type);
        Assert.Equal(3, dto.PlanetId);
    }

    [Fact]
    public void PlanetPublishedDto_ToModel_MapsExternalId()
    {
        var dto = new PlanetPublishedDto { Id = 42, Name = "Jupiter", Event = "Planet_Published" };

        var model = dto.ToModel();

        Assert.Equal(42, model.ExternalId);
        Assert.Equal("Jupiter", model.Name);
    }

    [Fact]
    public void Planets_ToReadDtos_MapsCollection()
    {
        var planets = new List<Planet>
        {
            new() { Id = 1, ExternalId = 10, Name = "Earth" },
            new() { Id = 2, ExternalId = 20, Name = "Mars" }
        };

        var dtos = planets.ToReadDtos().ToList();

        Assert.Equal(2, dtos.Count);
        Assert.Equal(10, dtos[0].ExternalId);
    }
}
