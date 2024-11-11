using System.ComponentModel.DataAnnotations;

namespace SatelliteService.Models;

public record Planet
{
    [Key]
    [Required]
    public int Id { get; init; }

    [Required]
    public int ExternalId { get; init; }

    [Required]
    public string? Name { get; init; }

    public ICollection<Satellite> Satellites { get; init; } = new List<Satellite>();
}