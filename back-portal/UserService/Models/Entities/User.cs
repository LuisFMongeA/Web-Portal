using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Entities;

public class User
{
    [EmailAddress]
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
