namespace SatelliteService.DTOs;

public record PlanetPublishedDto
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Event { get; init; }
}
