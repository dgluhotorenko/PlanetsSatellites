using PlanetService.DTOs;
using PlanetService.Models;

namespace PlanetService.Mappers;

public static class PlanetMapperExtensions
{
    public static IEnumerable<PlanetReadDto> ToReadDtos(this IEnumerable<Planet> planets) =>
        planets.Select(planet => planet.ToReadDto());

    public static PlanetReadDto ToReadDto(this Planet planet) =>
        new()
        {
            Id = planet.Id,
            Name = planet.Name,
            Mass = planet.Mass,
            Radius = planet.Radius
        };

    public static Planet ToModel(this PlanetCreateDto dto) =>
        new()
        {
            Name = dto.Name,
            Mass = dto.Mass,
            Radius = dto.Radius
        };

    public static PlanetPublishedDto ToPublishedDto(this PlanetReadDto dto) =>
        new()
        {
            Id = dto.Id,
            Name = dto.Name,
            Event = MessageBusConstants.PlanetPublishedEvent
        };

    public static GrpcPlanetModel ToGrpcModel(this Planet planet) =>
        new()
        {
            Id = planet.Id,
            Name = planet.Name,
            Mass = planet.Mass,
            Radius = planet.Radius
        };
}
