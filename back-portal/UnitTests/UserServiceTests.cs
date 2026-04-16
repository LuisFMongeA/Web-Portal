using Moq;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Models.Interfaces;

namespace UnitTests;

public class UserServiceTests
{
    private readonly UserService.Services.UserService _sut;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IAuthClient> _authClient;

    public UserServiceTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _authClient = new Mock<IAuthClient>();
        _sut = new UserService.Services.UserService(_userRepository.Object, _authClient.Object);
    }

    [Fact]
    public async Task CreateUser_ValidEmail_CallsRepositoryAddAsync()
    {
        _userRepository
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        await _sut.CreateUser(new CreateUserDto { Email = "test@test.com" });

        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ThrowsArgumentException()
    {
        _userRepository
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .ThrowsAsync(new ArgumentException("Email already exists"));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.CreateUser(new CreateUserDto { Email = "test@test.com" }));
    }

    [Fact]
    public async Task LoginUser_ValidEmail_ReturnsResponseDto()
    {
        var user = new User { Email = "test@test.com" };
        var expectedResponse = new ResponseDto
        {
            Email = "test@test.com",
            AccessToken = "token",
            RefreshToken = "refresh"
        };

        _userRepository
            .Setup(r => r.FindAsync("test@test.com"))
            .ReturnsAsync(user);

        _authClient
            .Setup(a => a.GetTokenAsync("test@test.com"))
            .ReturnsAsync(expectedResponse);

        var result = await _sut.LoginUser(new LoginUserDto { Email = "test@test.com" });

        Assert.NotNull(result);
        Assert.Equal("test@test.com", result.Email);
        Assert.Equal("token", result.AccessToken);
    }

    [Fact]
    public async Task LoginUser_UserNotFound_ThrowsArgumentException()
    {
        _userRepository
            .Setup(r => r.FindAsync("unknown@test.com"))
            .ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _sut.LoginUser(new LoginUserDto { Email = "unknown@test.com" }));
    }

    [Fact]
    public async Task LoginUser_ValidEmail_CallsAuthClientGetTokenAsync()
    {
        var user = new User { Email = "test@test.com" };

        _userRepository
            .Setup(r => r.FindAsync("test@test.com"))
            .ReturnsAsync(user);

        _authClient
            .Setup(a => a.GetTokenAsync("test@test.com"))
            .ReturnsAsync(new ResponseDto { Email = "test@test.com" });

        await _sut.LoginUser(new LoginUserDto { Email = "test@test.com" });

        _authClient.Verify(a => a.GetTokenAsync("test@test.com"), Times.Once);
    }
}

