using PlanetService.DTOs;
using PlanetService.Models;

namespace PlanetService.Mappers;

// use AutoMapper when it will be really needed
public static class PlanetMapperExtensions
{
    // IEnumerable<Planet> -> IEnumerable<PlanetReadDto>
    public static IEnumerable<PlanetReadDto> ToReadDtos(this IEnumerable<Planet> planets) =>
        planets.Select(planet => planet.ToReadDto());

    // Planet -> PlanetReadDto
    public static PlanetReadDto ToReadDto(this Planet planet) =>
        new()
        {
            Id = planet.Id,
            Name = planet.Name,
            Mass = planet.Mass,
            Radius = planet.Radius
        };

    // PlanetCreateDto -> Planet
    public static Planet ToModel(this PlanetCreateDto planetCreateDto) =>
        new()
        {
            Name = planetCreateDto.Name,
            Mass = planetCreateDto.Mass,
            Radius = planetCreateDto.Radius
        };

    // PlanetReadDto -> PlanetPublishedDto
    public static PlanetPublishedDto ToPublishedDto(this PlanetReadDto planetReadDto) =>
        new()
        {
            Id = planetReadDto.Id,
            Name = planetReadDto.Name
        };
}