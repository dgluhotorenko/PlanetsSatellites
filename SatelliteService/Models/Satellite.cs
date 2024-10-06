using System.ComponentModel.DataAnnotations;

namespace SatelliteService.Models;

public record Satellite
{
    [Key]
    [Required]
    public required int Id { get; init; }

    [Required]
    public required string Name { get; init; }

    public required string Type { get; init; }

    public required string Orbit { get; init; }
}