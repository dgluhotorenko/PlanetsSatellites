using SatelliteService.DTOs;
using SatelliteService.Models;

namespace SatelliteService.Mappers;

// use AutoMapper when it will be really needed
public static class SatelliteMapperExtensions
{
    // IEnumerable<Planet> -> IEnumerable<PlanetReadDto>
    public static IEnumerable<PlanetReadDto> ToReadDtos(this IEnumerable<Planet> planets) =>
        planets.Select(p => p.ToReadDto());

    // Planet -> PlanetReadDto
    private static PlanetReadDto ToReadDto(this Planet planet) =>
        new()
        {
            Id = planet.Id,
            ExternalId = planet.ExternalId,
            Name = planet.Name
        };

    // SatelliteCreateDto -> Satellite
    public static Satellite ToModel(this SatelliteCreateDto satelliteCreateDto) =>
        new()
        {
            Name = satelliteCreateDto.Name,
            Type = satelliteCreateDto.Type
        };

    // IEnumerable<Satellite> -> IEnumerable<SatelliteReadDto>
    public static IEnumerable<SatelliteReadDto> ToReadDtos(this IEnumerable<Satellite> satellites) =>
        satellites.Select(s => s.ToReadDto());

    // Satellite -> SatelliteReadDto
    public static SatelliteReadDto ToReadDto(this Satellite satellite) =>
        new()
        {
            Id = satellite.Id,
            Name = satellite.Name,
            Type = satellite.Type,
            PlanetId = satellite.PlanetId
        };

    // PlanetPublishedDto -> Planet
    public static Planet ToModel(this PlanetPublishedDto? planetPublishedDto) =>
        new()
        {
            ExternalId = planetPublishedDto!.Id,
            Name = planetPublishedDto.Name
        };
}