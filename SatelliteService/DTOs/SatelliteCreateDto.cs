namespace SatelliteService.DTOs;

public record SatelliteCreateDto
{
    public required string Name { get; init; }

    public required string Type { get; init; }

    public required string Orbit { get; init; }
}