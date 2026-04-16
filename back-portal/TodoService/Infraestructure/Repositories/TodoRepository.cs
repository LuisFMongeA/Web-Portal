using TodoService.Models.Entities;
using TodoService.Models.Interfaces;

namespace TodoService.Infraestructure.Repositories;

public class TodoRepository : ITodoRepository
{
    Dictionary<string, List<TodoItem>> todoDictionary;
    public TodoRepository() 
    {
        todoDictionary = new Dictionary<string, List<TodoItem>>();
    }
    public Task<TodoItem> AddAsync(TodoItem item)
    {
        if (!todoDictionary.ContainsKey(item.UserId))
            todoDictionary[item.UserId] = new List<TodoItem>();

        item.CreatedAt = DateTime.UtcNow;
        todoDictionary[item.UserId].Add(item);
        return Task.FromResult(item);
    }

    public Task<IEnumerable<TodoItem>> GetAllAsync(string email)
    {
        if (!todoDictionary.TryGetValue(email, out var todosList))
            throw new ArgumentException("User not found");

        return Task.FromResult(todosList.AsEnumerable());
    }

    public  Task<TodoItem> GetByIdAsync(Guid id, string email)
    {
        return Task.FromResult(FindById(id, email));
    }

    public Task RemoveAsync(Guid id, string email)
    {
        if (!todoDictionary.TryGetValue(email, out var todosList))
            throw new ArgumentException("Invalid user");

        var todoItem = FindById(id, email);

        if (!todosList.Remove(todoItem))
            throw new Exception("Removing error");

        return Task.CompletedTask;
    }

    public Task<TodoItem> UpdateAsync(TodoItem item)
    {
        var todoItem = FindById(item.Id , item.UserId);

        todoItem.Description = item.Description;
        todoItem.UpdatedAt = DateTime.UtcNow;

        return Task.FromResult(todoItem);
    }

    public Task<TodoItem> UpdateDoneAsync(Guid id, string email, bool newValue)
    { 
        var todoItem = FindById(id, email);
        todoItem.Done = newValue;
        todoItem.UpdatedAt=DateTime.UtcNow;

        return Task.FromResult(todoItem);
    }

    private TodoItem FindById(Guid id, string email) 
    {
        if (!todoDictionary.TryGetValue(email, out var todosList))
            throw new ArgumentException("Invalid user");

        var todoItem = todosList.FirstOrDefault(t => t.Id == id);
        if (todoItem == null)
            throw new ArgumentException("TodoItem not found for user");

        return todoItem;
    }
}
