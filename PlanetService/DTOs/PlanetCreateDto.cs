using System.ComponentModel.DataAnnotations;

namespace PlanetService.DTOs;

public record PlanetCreateDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; init; }

    [Range(0, double.MaxValue, ErrorMessage = "Mass must be a positive value.")]
    public required double Mass { get; init; }

    [Range(0, double.MaxValue, ErrorMessage = "Radius must be a positive value.")]
    public required double Radius { get; init; }
}
