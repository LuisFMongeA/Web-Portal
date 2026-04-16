using System.ComponentModel.DataAnnotations;

namespace TodoService.Models.DTOs;

public class UpdateTodoDto
{
    [Required]
    public required Guid Id { get; set; }
    public required string UserId { get; set; }
    [Required]
    [MaxLength(250)]
    public required string Description { get; set; }
}
