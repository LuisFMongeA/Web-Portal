using System.ComponentModel.DataAnnotations;

namespace UserService.Models.DTOs;

public class LoginUserDto
{
    [Required]
    [MaxLength(250)]
    public required string Email { get; set; } = String.Empty;
}
