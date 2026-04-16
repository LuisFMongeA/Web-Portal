using System.ComponentModel.DataAnnotations;

namespace TodoService.Models.DTOs;

public class CreateTodoDto
{
    public required string UserId { get; set; }
    [Required]
    [MaxLength(250)]
    public required string Description { get; set; }

}
