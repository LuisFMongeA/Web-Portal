namespace TodoService.Models.DTOs;

public class ResponseTodoDto
{
    public Guid Id { get; set; }
    public required string UserId { get; set; }
    public required string Description { get; set; }
    public bool Done { get; set; } = false;

}
