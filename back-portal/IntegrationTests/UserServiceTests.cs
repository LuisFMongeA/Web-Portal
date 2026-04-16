using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UserService.Models.DTOs;
using UserService.Models.Interfaces;
using Xunit;

namespace IntegrationTests;


public class UserServiceIntegrationTests : IClassFixture<WebApplicationFactory<UserService.Program>>
{
    private readonly HttpClient _client;
    private readonly Mock<IAuthClient> _mockAuthClient;

    public UserServiceIntegrationTests(WebApplicationFactory<UserService.Program> factory)
    {
        _mockAuthClient = new Mock<IAuthClient>();
        _mockAuthClient
            .Setup(a => a.GetTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(new ResponseDto
            {
                Email = "test@test.com",
                AccessToken = "accesstoken",
                RefreshToken = "refreshtoken",
                ExpiresIn = 900,
                TokenType = "Bearer"
            });

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IAuthClient));
                if (descriptor != null) services.Remove(descriptor);
                services.AddSingleton(_mockAuthClient.Object);
            });
        }).CreateClient();
    }

    [Fact]
    public async Task CreateUser_ValidEmail_Returns200()
    {
        var email = $"test_{Guid.NewGuid()}@test.com";
        var response = await _client.PostAsJsonAsync("/api/user/create",
            new CreateUserDto { Email = email });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_Returns409()
    {
        var email = $"test_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/user/create",
            new CreateUserDto { Email = email });

        var response = await _client.PostAsJsonAsync("/api/user/create",
            new CreateUserDto { Email = email });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task CreateUser_InvalidEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/user/create",
            new CreateUserDto { Email = "notanemail" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task LoginUser_ValidEmail_Returns200WithTokens()
    {
        var email = $"test_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/user/create",
            new CreateUserDto { Email = email });

        _mockAuthClient
            .Setup(a => a.GetTokenAsync(email))
            .ReturnsAsync(new ResponseDto
            {
                Email = email,
                AccessToken = "accesstoken",
                RefreshToken = "refreshtoken",
                ExpiresIn = 900,
                TokenType = "Bearer"
            });

        var response = await _client.PostAsJsonAsync("/api/user/login",
            new LoginUserDto { Email = email });

        var result = await response.Content.ReadFromJsonAsync<ResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.NotEmpty(result.AccessToken);
    }

    [Fact]
    public async Task LoginUser_UserNotFound_Returns404()
    {
        var response = await _client.PostAsJsonAsync("/api/user/login",
            new LoginUserDto { Email = $"unknown_{Guid.NewGuid()}@test.com" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task LoginUser_ValidEmail_CallsAuthClient()
    {
        var email = $"test_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/user/create",
            new CreateUserDto { Email = email });

        await _client.PostAsJsonAsync("/api/user/login",
            new LoginUserDto { Email = email });

        _mockAuthClient.Verify(a => a.GetTokenAsync(email), Times.Once);
    }
}
