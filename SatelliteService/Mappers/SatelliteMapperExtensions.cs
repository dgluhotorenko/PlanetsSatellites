using PlanetService;
using SatelliteService.DTOs;
using SatelliteService.Models;

namespace SatelliteService.Mappers;

public static class SatelliteMapperExtensions
{
    public static IEnumerable<PlanetReadDto> ToReadDtos(this IEnumerable<Planet> planets) =>
        planets.Select(p => p.ToReadDto());

    private static PlanetReadDto ToReadDto(this Planet planet) =>
        new()
        {
            Id = planet.Id,
            ExternalId = planet.ExternalId,
            Name = planet.Name
        };

    public static Satellite ToModel(this SatelliteCreateDto dto) =>
        new()
        {
            Name = dto.Name,
            Type = dto.Type
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

    public static Planet ToModel(this PlanetPublishedDto dto) =>
        new()
        {
            ExternalId = dto.Id,
            Name = dto.Name
        };

    public static IEnumerable<Planet> ToModels(this IEnumerable<GrpcPlanetModel> grpcModels) =>
        grpcModels.Select(p => p.ToModel());

    private static Planet ToModel(this GrpcPlanetModel grpcModel) =>
        new()
        {
            ExternalId = grpcModel.Id,
            Name = grpcModel.Name
        };
}
