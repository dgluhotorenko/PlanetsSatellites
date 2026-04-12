using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
    public required string Password { get; init; }
}
