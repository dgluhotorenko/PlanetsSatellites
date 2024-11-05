namespace PlanetService.DTOs;

public record PlanetReadDto
{
    public required int Id { get; init; }

    public required string? Name { get; init; }

    public required double Mass { get; init; }

    public required double Radius { get; init; }
}