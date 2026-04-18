using System.ComponentModel.DataAnnotations;

namespace PlanetsSatellites.Web.Models;

public sealed class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(10)]
    public string Password { get; set; } = string.Empty;

    [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public sealed class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed record LoginResponse(string Token, DateTime Expiration);

public sealed record PlanetDto(int Id, string Name, double Mass, double Radius);

public sealed class CreatePlanetRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0.001, double.MaxValue, ErrorMessage = "Mass must be positive.")]
    public double Mass { get; set; }

    [Range(0.001, double.MaxValue, ErrorMessage = "Radius must be positive.")]
    public double Radius { get; set; }
}

public sealed record SatelliteDto(int Id, string Name, string Type, int PlanetId);

public sealed class CreateSatelliteRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Type { get; set; } = "Natural";
}

/// <summary>Planet as seen by SatelliteService (replicated via RabbitMQ). Id is local; ExternalId is PlanetService's Id.</summary>
public sealed record ReplicatedPlanetDto(int Id, int ExternalId, string Name);
