using SatelliteService.DTOs;
using SatelliteService.Models;

namespace SatelliteService.Mappers;

// use AutoMapper when it will be really needed
public static class SatelliteMapperExtensions
{
    public static IEnumerable<PlanetReadDto> ToReadDtos(this IEnumerable<Planet> planets) =>
        planets.Select(p => p.ToReadDto());

    private static PlanetReadDto ToReadDto(this Planet planet) =>
        new()
        {
            Id = planet.Id,
            Name = planet.Name
        };

    public static Satellite ToModel(this SatelliteCreateDto satelliteCreateDto) =>
        new()
        {
            Name = satelliteCreateDto.Name,
            Type = satelliteCreateDto.Type
        };
    public static IEnumerable<SatelliteReadDto> ToReadDtos(this IEnumerable<Satellite> satellites) =>
        satellites.Select(s => s.ToReadDto());

    public static SatelliteReadDto ToReadDto(this Satellite satellite) =>
        new()
        {
            Id = satellite.Id,
            Name = satellite.Name,
            Type = satellite.Type,
            PlanetId = satellite.PlanetId
        };
}