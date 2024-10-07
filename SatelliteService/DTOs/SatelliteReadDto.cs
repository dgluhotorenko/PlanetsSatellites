namespace SatelliteService.DTOs;

public record SatelliteReadDto
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Type { get; init; }

    public required string Orbit { get; init; }
}