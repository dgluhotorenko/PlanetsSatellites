using System.ComponentModel.DataAnnotations;

namespace SatelliteService.DTOs;

public record SatelliteCreateDto
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; init; }

    [Required]
    [MaxLength(50)]
    public required string Type { get; init; }
}
