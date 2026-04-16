using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs;

public class CreateUserDto
{
    [Required]
    [MaxLength(250)]
    [EmailAddress]
    public required string Email { get; set; } = String.Empty;
}
