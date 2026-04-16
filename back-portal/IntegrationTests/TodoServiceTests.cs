using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TodoService.Models.DTOs;
using TodoService.Models.Interfaces;
using Xunit;

namespace IntegrationTests;

public class TodoServiceIntegrationTests : IClassFixture<WebApplicationFactory<TodoService.Program>>
{
    private readonly HttpClient _client;
    private const string TestEmail = "test@test.com";
    private const string TestToken = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJ0ZXN0QHRlc3QuY29tIiwic3ViIjoidGVzdEB0ZXN0LmNvbSIsImlhdCI6IjE3NzYzMjcxNDUiLCJleHAiOjE3NzYzMjgwNDUsImlzcyI6IkF1dGhTZXJ2aWNlIiwiYXVkIjoiVG9kb1BvcnRhbCJ9.jQ67aBc0m0a9c22Ni77dc6QV84eGUySbg4Y62YGPS4Ax-wU4GOe2Ds5R2c8UqwxoFHIwYfacFAXfKgOaxFRsaPcaYVjcn68tfNxP1dQJLDw3hsFE8UvustwIaUaDo3rWVnTcOUG3t-zaiLDF76klceLOSrBDQSBXXIyc_nMH-4T8H7r4fR-auvsZdxiCi2ZNudVdYHXPBckDdp-WnPIuPXEJ5LnDCEIYroTPp1ixUTotL_JaOvQ4vnt_5boN1-X3kLNUccevFFY1J57fTY6cHz5sd1WV0EiRNztmh9-lzytuvgNn4v6-cCXgllBwxPsOOwqbBSscAM-aau2pvcwrhA";

    public TodoServiceIntegrationTests(WebApplicationFactory<TodoService.Program> factory)
    {
        var mockAuthClient = new Mock<IAuthClient>();
        mockAuthClient
            .Setup(a => a.IsTokenValid(It.IsAny<string>()))
            .ReturnsAsync(true);

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthClient));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton(mockAuthClient.Object);
            });
        }).CreateClient();

        _client.DefaultRequestHeaders.Add("Authorization", TestToken);
    }

    [Fact]
    public async Task CreateTodo_ValidDescription_Returns201()
    {
        var response = await _client.PostAsJsonAsync("/api/todo",
            new CreateTodoDto { UserId = TestEmail, Description = "Test todo" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_EmptyDescription_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/todo",
            new CreateTodoDto { UserId = TestEmail, Description = "" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateTodo_DescriptionTooLong_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/todo",
            new CreateTodoDto { UserId = TestEmail, Description = new string('a', 251) });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAllTodos_ExistingUser_Returns200()
    {
        await _client.PostAsJsonAsync("/api/todo",
            new CreateTodoDto { UserId = TestEmail, Description = "Test todo" });

        var response = await _client.GetAsync("/api/todo");

        var result = await response.Content.ReadFromJsonAsync<IEnumerable<ResponseTodoDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

  
    [Fact]
    public async Task GetTodoById_ExistingTodo_Returns200()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todo",
            new CreateTodoDto { UserId = TestEmail, Description = "Test todo" });
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseTodoDto>();

        var response = await _client.GetAsync($"/api/todo/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTodoById_NonExistingTodo_Returns404()
    {
        var response = await _client.GetAsync($"/api/todo/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateTodo_ValidData_Returns200()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todo",
            new CreateTodoDto { UserId = TestEmail, Description = "Original" });
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseTodoDto>();

        var response = await _client.PutAsJsonAsync($"/api/todo/{created!.Id}",
            new UpdateTodoDto { Id = created.Id, UserId = TestEmail, Description = "Updated" });

        var result = await response.Content.ReadFromJsonAsync<ResponseTodoDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Updated", result!.Description);
    }

    [Fact]
    public async Task UpdateTodo_MismatchedId_Returns400()
    {
        var response = await _client.PutAsJsonAsync($"/api/todo/{Guid.NewGuid()}",
            new UpdateTodoDto { Id = Guid.NewGuid(), UserId = TestEmail, Description = "Updated" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_ExistingTodo_Returns204()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todo",
            new CreateTodoDto { UserId = TestEmail, Description = "To delete" });
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseTodoDto>();

        var response = await _client.DeleteAsync($"/api/todo/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteTodo_NonExistingTodo_Returns404()
    {
        var response = await _client.DeleteAsync($"/api/todo/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDone_ExistingTodo_Returns200WithUpdatedValue()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todo",
            new CreateTodoDto { UserId = TestEmail, Description = "Test todo" });
        var created = await createResponse.Content.ReadFromJsonAsync<ResponseTodoDto>();

        var response = await _client.PatchAsync($"/api/todo/{created!.Id}/done?newValue=true", null);
        var result = await response.Content.ReadFromJsonAsync<ResponseTodoDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(result!.Done);
    }

    [Fact]
    public async Task UpdateDone_NonExistingTodo_Returns404()
    {
        var response = await _client.PatchAsync($"/api/todo/{Guid.NewGuid()}/done?newValue=true", null);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
