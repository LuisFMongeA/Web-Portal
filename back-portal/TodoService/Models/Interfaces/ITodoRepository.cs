using TodoService.Models.Entities;

namespace TodoService.Models.Interfaces;

public interface ITodoRepository
{
    Task<TodoItem> AddAsync(TodoItem item);
    Task RemoveAsync(Guid id, string email);
    Task<TodoItem> UpdateAsync(TodoItem item);
    Task<TodoItem> UpdateDoneAsync(Guid id, string email, bool newValue);
    Task<TodoItem> GetByIdAsync(Guid id, string email);
    Task<IEnumerable<TodoItem>> GetAllAsync(string email);

}
