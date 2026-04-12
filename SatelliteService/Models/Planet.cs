using System.ComponentModel.DataAnnotations;

namespace SatelliteService.Models;

public record Planet
{
    [Key]
    public int Id { get; init; }

    [Required]
    public int ExternalId { get; init; }

    [Required]
    [MaxLength(100)]
    public required string Name { get; init; }

    public ICollection<Satellite> Satellites { get; init; } = new List<Satellite>();
}
