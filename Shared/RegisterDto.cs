using System.ComponentModel.DataAnnotations;

namespace Rafeeq.Api.DTOs;

public class RegisterDto
{
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(Patient|Doctor)$", ErrorMessage = "Role must be either 'Patient' or 'Doctor'")]
    public string Role { get; set; } = string.Empty;

    // Patient-specific fields
    public int? Age { get; set; }
    public string? Gender { get; set; }

    // Doctor-specific fields
    public string? Specialization { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
}
