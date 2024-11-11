using System.ComponentModel.DataAnnotations;

namespace SatelliteService.DTOs;

public record SatelliteCreateDto
{
    [Required]
    public string? Name { get; init; }

    [Required]
    public string? Type { get; init; }
}