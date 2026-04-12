using System.ComponentModel.DataAnnotations;

namespace SatelliteService.Models;

public record Satellite
{
    [Key]
    public int Id { get; init; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; init; }

    [Required]
    [MaxLength(50)]
    public required string Type { get; init; }

    [Required]
    public int PlanetId { get; set; }

    public Planet? Planet { get; init; }
}
