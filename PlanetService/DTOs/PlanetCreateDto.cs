namespace PlanetService.DTOs;

public record PlanetCreateDto
{
    public required string Name { get; init; }

    public required double Mass { get; init; }

    public required double Radius { get; init; }
}