using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
