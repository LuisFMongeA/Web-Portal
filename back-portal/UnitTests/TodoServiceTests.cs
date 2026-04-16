using Moq;
using TodoService.Models.DTOs;
using TodoService.Models.Entities;
using TodoService.Models.Interfaces;

namespace UnitTests;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _mockRepo;
    private readonly TodoService.Services.TodoService _sut;

    public TodoServiceTests()
    {
        _mockRepo = new Mock<ITodoRepository>();
        _sut = new TodoService.Services.TodoService(_mockRepo.Object);
    }

    [Fact]
    public async Task CreateTodo_ValidDto_ReturnsResponseDto()
    {
        var dto = new CreateTodoDto { UserId = "test@test.com", Description = "Test todo" };
        _mockRepo.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
                 .ReturnsAsync((TodoItem item) => item);

        var result = await _sut.CreateTodo(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Description, result.Description);
        Assert.Equal(dto.UserId, result.UserId);
        Assert.False(result.Done);
    }

    [Fact]
    public async Task GetAll_ExistingUser_ReturnsListOfDtos()
    {
        var email = "test@test.com";
        var items = new List<TodoItem>
        {
            new TodoItem { UserId = email, Description = "Todo 1" },
            new TodoItem { UserId = email, Description = "Todo 2" }
        };
        _mockRepo.Setup(r => r.GetAllAsync(email))
                 .ReturnsAsync(items);

        var result = await _sut.GetAll(email);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetAll_NonExistingUser_ThrowsArgumentException()
    {
        _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<string>()))
                 .ThrowsAsync(new ArgumentException("User not found"));

        await Assert.ThrowsAsync<ArgumentException>(() => _sut.GetAll("nouser@test.com"));
    }

    [Fact]
    public async Task GetTodoById_ExistingTodo_ReturnsResponseDto()
    {
        var email = "test@test.com";
        var item = new TodoItem { UserId = email, Description = "Test todo" };
        _mockRepo.Setup(r => r.GetByIdAsync(item.Id, email))
                 .ReturnsAsync(item);

        var result = await _sut.GetTodoById(item.Id, email);

        Assert.NotNull(result);
        Assert.Equal(item.Description, result.Description);
        Assert.Equal(item.Id, result.Id);
    }

    [Fact]
    public async Task GetTodoById_NonExistingTodo_ThrowsArgumentException()
    {
        _mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                 .ThrowsAsync(new ArgumentException("TodoItem not found for user"));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.GetTodoById(Guid.NewGuid(), "test@test.com"));
    }

    [Fact]
    public async Task UpdateTodo_ValidDto_ReturnsUpdatedResponseDto()
    {
        var email = "test@test.com";
        var id = Guid.NewGuid();
        var dto = new UpdateTodoDto { Id = id, UserId = email, Description = "Updated" };
        var updatedItem = new TodoItem { Id = id, UserId = email, Description = "Updated" };
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
                 .ReturnsAsync(updatedItem);

        var result = await _sut.UpdateTodo(dto);

        Assert.NotNull(result);
        Assert.Equal("Updated", result.Description);
    }

    [Fact]
    public async Task DeleteTodo_NonExistingTodo_ThrowsArgumentException()
    {
        _mockRepo.Setup(r => r.RemoveAsync(It.IsAny<Guid>(), It.IsAny<string>()))
                 .ThrowsAsync(new ArgumentException("TodoItem not found for user"));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.DeleteTodo(Guid.NewGuid(), "test@test.com"));
    }

    [Fact]
    public async Task UpdateDone_ValidTodo_ReturnsDtoWithUpdatedDone()
    {
        var id = Guid.NewGuid();
        var email = "test@test.com";
        var updatedItem = new TodoItem { Id = id, UserId = email, Description = "Test", Done = true };
        _mockRepo.Setup(r => r.UpdateDoneAsync(id, email, true))
                 .ReturnsAsync(updatedItem);

        var result = await _sut.UpdateDone(id, email, true);

        Assert.True(result.Done);
    }
}