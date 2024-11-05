using PlanetService.DTOs;
using PlanetService.Models;

namespace PlanetService.Mappers;

// use AutoMapper when it will be really needed
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
    
    public static Planet ToModel(this PlanetCreateDto planetCreateDto) =>
        new()
        {
            Name = planetCreateDto.Name,
            Mass = planetCreateDto.Mass,
            Radius = planetCreateDto.Radius
        };
}