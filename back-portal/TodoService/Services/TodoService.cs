using TodoService.Models.DTOs;
using TodoService.Models.Entities;
using TodoService.Models.Interfaces;

namespace TodoService.Services;

public class TodoService(ITodoRepository todoRepository) : ITodoService
{
    public async Task<ResponseTodoDto> CreateTodo(CreateTodoDto todoItemDto)
    {
        TodoItem item = new TodoItem
        {
            Description = todoItemDto.Description,
            UserId = todoItemDto.UserId,
        };
        await todoRepository.AddAsync(item);

        return new ResponseTodoDto
        {
            Id = item.Id,
            UserId = item.UserId,
            Description = item.Description,
            Done = item.Done,
        };
    }

    public async Task DeleteTodo(Guid todoId, string email)
    {
        await todoRepository.RemoveAsync(todoId, email);
    }

    public async Task<IEnumerable<ResponseTodoDto>> GetAll(string email)
    {
        var list = await todoRepository.GetAllAsync(email);
        return list.Select(s => new ResponseTodoDto
        {
            Id = s.Id,
            Description = s.Description,
            UserId = s.UserId,
            Done = s.Done
        });
    }

    public async Task<ResponseTodoDto> GetTodoById(Guid id, string email)
    {
        var item = await todoRepository.GetByIdAsync(id, email);
        return new ResponseTodoDto
        {
            Id = item.Id,
            Description = item.Description,
            UserId = item.UserId,
            Done = item.Done
        };
    }

    public async Task<ResponseTodoDto> UpdateDone(Guid todoId, string email, bool newValue)
    {
        var item = await todoRepository.UpdateDoneAsync(todoId, email, newValue);
        return new ResponseTodoDto
        {
            Id = item.Id,
            Description = item.Description,
            UserId = item.UserId,
            Done = item.Done
        };
    }

    public async Task<ResponseTodoDto> UpdateTodo(UpdateTodoDto todoItemDto)
    {
        TodoItem item = new TodoItem
        {
            Id = todoItemDto.Id,
            Description = todoItemDto.Description,
            UserId = todoItemDto.UserId,
            
        };

         var res = await todoRepository.UpdateAsync(item);

        return new ResponseTodoDto
        {
            Id = res.Id,
            Description = res.Description,
            UserId = res.UserId,
            Done = res.Done
        };
    }

    
}
