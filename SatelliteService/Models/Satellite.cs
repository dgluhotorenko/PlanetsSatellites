using System.ComponentModel.DataAnnotations;

namespace SatelliteService.Models;

public record Satellite
{
    [Key]
    [Required]
    public int Id { get; init; }

    [Required]
    public string? Name { get; init; }

    public string? Type { get; init; }

    public string? Orbit { get; init; }
}