using TodoService.Models.DTOs;

namespace TodoService.Models.Interfaces;

public interface ITodoService
{
    Task<ResponseTodoDto> CreateTodo(CreateTodoDto todoItemDto);
    Task<ResponseTodoDto> UpdateTodo(UpdateTodoDto todoItemDto);
    Task DeleteTodo(Guid todoId, string email);
    Task<ResponseTodoDto> UpdateDone(Guid todoId, string email, bool newValue);
    Task<ResponseTodoDto> GetTodoById(Guid id, string email);
    Task<IEnumerable<ResponseTodoDto>> GetAll(string email);
}
