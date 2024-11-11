namespace SatelliteService.DTOs;

public record SatelliteReadDto
{
    public int Id { get; init; }

    public string? Name { get; init; }

    public string? Type { get; init; }

    public int PlanetId { get; set; }
}