namespace TodoService.Models.Entities;

public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string UserId { get; set; }
    public required string Description { get; set; }
    public bool Done { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}