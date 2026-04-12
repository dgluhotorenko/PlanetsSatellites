namespace SatelliteService.DTOs;

public record PlanetReadDto
{
    public required int Id { get; init; }

    public required int ExternalId { get; init; }

    public required string Name { get; init; }
}
