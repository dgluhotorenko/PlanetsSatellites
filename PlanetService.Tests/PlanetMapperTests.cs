using PlanetService.DTOs;
using PlanetService.Mappers;
using PlanetService.Models;

namespace PlanetService.Tests;

public class PlanetMapperTests
{
    [Fact]
    public void ToReadDto_MapsAllProperties()
    {
        var planet = new Planet { Id = 1, Name = "Earth", Mass = 1.0, Radius = 6371.0 };

        var dto = planet.ToReadDto();

        Assert.Equal(1, dto.Id);
        Assert.Equal("Earth", dto.Name);
        Assert.Equal(1.0, dto.Mass);
        Assert.Equal(6371.0, dto.Radius);
    }

    [Fact]
    public void ToModel_MapsFromCreateDto()
    {
        var dto = new PlanetCreateDto { Name = "Mars", Mass = 0.107, Radius = 3389.5 };

        var model = dto.ToModel();

        Assert.Equal("Mars", model.Name);
        Assert.Equal(0.107, model.Mass);
        Assert.Equal(3389.5, model.Radius);
    }

    [Fact]
    public void ToPublishedDto_SetsEventType()
    {
        var readDto = new PlanetReadDto { Id = 1, Name = "Earth", Mass = 1.0, Radius = 6371.0 };

        var published = readDto.ToPublishedDto();

        Assert.Equal(1, published.Id);
        Assert.Equal("Earth", published.Name);
        Assert.Equal(MessageBusConstants.PlanetPublishedEvent, published.Event);
    }

    [Fact]
    public void ToReadDtos_MapsCollection()
    {
        var planets = new List<Planet>
        {
            new() { Id = 1, Name = "Earth", Mass = 1.0, Radius = 6371.0 },
            new() { Id = 2, Name = "Mars", Mass = 0.107, Radius = 3389.5 }
        };

        var dtos = planets.ToReadDtos().ToList();

        Assert.Equal(2, dtos.Count);
        Assert.Equal("Earth", dtos[0].Name);
        Assert.Equal("Mars", dtos[1].Name);
    }

    [Fact]
    public void ToGrpcModel_MapsAllProperties()
    {
        var planet = new Planet { Id = 1, Name = "Earth", Mass = 1.0, Radius = 6371.0 };

        var grpc = planet.ToGrpcModel();

        Assert.Equal(1, grpc.Id);
        Assert.Equal("Earth", grpc.Name);
        Assert.Equal(1.0, grpc.Mass);
        Assert.Equal(6371.0, grpc.Radius);
    }
}
