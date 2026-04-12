using System.ComponentModel.DataAnnotations;

namespace PlanetService.Models;

public record Planet
{
    [Key]
    public int Id { get; init; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; init; }

    /// <summary>Mass in Earth masses.</summary>
    [Range(0, double.MaxValue)]
    public double Mass { get; init; }

    /// <summary>Radius in kilometers.</summary>
    [Range(0, double.MaxValue)]
    public double Radius { get; init; }
}
