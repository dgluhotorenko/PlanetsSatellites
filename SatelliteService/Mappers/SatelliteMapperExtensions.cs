using SatelliteService.DTOs;
using SatelliteService.Models;

namespace SatelliteService.Mappers;

// use AutoMapper when it will be really needed
public static class SatelliteMapperExtensions
{
    public static IEnumerable<SatelliteReadDto> ToReadDtos(this IEnumerable<Satellite> satellites) =>
        satellites.Select(satellite => satellite.ToReadDto());

    private static SatelliteReadDto ToReadDto(this Satellite satellite) =>
        new()
        {
            Id = satellite.Id,
            Name = satellite.Name,
            Type = satellite.Type,
            Orbit = satellite.Orbit
        };
}