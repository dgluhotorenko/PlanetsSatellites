using System.ComponentModel.DataAnnotations;

namespace SatelliteService.Models;

public record Satellite
{
    [Key]
    [Required]
    public int Id { get; init; }

    [Required]
    public string? Name { get; init; }

    [Required]
    public string? Type { get; init; }

    [Required]
    public int PlanetId { get; set; }

    public Planet? Planet { get; init; }
}