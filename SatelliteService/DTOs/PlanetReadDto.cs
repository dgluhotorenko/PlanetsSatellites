namespace SatelliteService.DTOs;

public record PlanetReadDto
{
    public int Id { get; init; }

    public string? Name { get; init; }
}