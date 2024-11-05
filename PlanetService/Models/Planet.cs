using System.ComponentModel.DataAnnotations;

namespace PlanetService.Models;

public record Planet
{
    [Key]
    [Required]
    public int Id { get; init; }

    [Required]
    public string? Name { get; init; }

    // Earth masses
    public double Mass { get; init; }

    // Kilometers
    public double Radius { get; init; }
}