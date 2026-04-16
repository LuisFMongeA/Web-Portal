using Microsoft.Extensions.Caching.Memory;

namespace UnitTests;

public class AuthServiceTests
{
    private readonly AuthService.Services.AuthService _service;
    private readonly IMemoryCache _memoryCache;

    public AuthServiceTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _service = new AuthService.Services.AuthService(_memoryCache);
    }

    [Fact]
    public void GenerateToken_ValidEmail_ReturnsTokenResponseDto()
    {
        var result = _service.GenerateToken("test@test.com");
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal(900, result.ExpiresIn);
        Assert.Equal("Bearer", result.TokenType);
    }

    [Fact]
    public void GenerateToken_InvalidEmail_ThrowsArgumentException()
    {
        Assert.ThrowsAsync<ArgumentException>(() => Task.FromResult(_service.GenerateToken("notanemail")));
    }

    [Fact]
    public void RefreshToken_ValidToken_ReturnsNewTokenResponseDto()
    {
        var original = _service.GenerateToken("test@test.com");
        var result = _service.RefreshToken(original.RefreshToken);
        Assert.NotNull(result);
        Assert.NotEqual(original.RefreshToken, result.RefreshToken);
    }

    [Fact]
    public void RefreshToken_InvalidToken_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => _service.RefreshToken("invalidtoken"));
    }

    [Fact]
    public void RefreshToken_AlreadyUsedToken_ThrowsArgumentException()
    {
        var original = _service.GenerateToken("test@test.com");
        _service.RefreshToken(original.RefreshToken);
        Assert.Throws<ArgumentException>(() => _service.RefreshToken(original.RefreshToken));
    }

    [Fact]
    public void ValidateToken_AfterInvalidation_ReturnsFalse()
    {
        var token = _service.GenerateToken("test@test.com");
        _service.InvalidateToken(token.AccessToken);
        Assert.False(_service.ValidateToken(token.AccessToken));
    }

    [Fact]
    public void ValidateToken_ValidToken_ReturnsTrue()
    {
        var token = _service.GenerateToken("test@test.com");
        Assert.True(_service.ValidateToken(token.AccessToken));
    }

    [Fact]
    public void ValidateToken_InvalidToken_ReturnsFalse()
    {
        Assert.False(_service.ValidateToken("invalidtoken"));
    }
}
    