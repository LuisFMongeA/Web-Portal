using System.Net;
using System.Net.Http.Json;
using AuthService.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace IntegrationTests;

public class AuthServiceIntegrationTests : IClassFixture<WebApplicationFactory<AuthService.Program>>
{
    private readonly HttpClient _client;

    public AuthServiceIntegrationTests(WebApplicationFactory<AuthService.Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GenerateToken_ValidEmail_Returns200WithTokens()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/token",
            new TokenRequestDto { Email = $"test_{Guid.NewGuid()}@test.com" });

        var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEmpty(result.AccessToken);
        Assert.NotEmpty(result.RefreshToken);
        Assert.Equal(900, result.ExpiresIn);
        Assert.Equal("Bearer", result.TokenType);
    }

    [Fact]
    public async Task GenerateToken_InvalidEmail_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/token",
            new TokenRequestDto { Email = "notanemail" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_Returns200WithNewTokens()
    {
        var tokenResponse = await _client.PostAsJsonAsync("/api/auth/token",
            new TokenRequestDto { Email = $"test_{Guid.NewGuid()}@test.com" });
        var original = await tokenResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto { RefreshToken = original!.RefreshToken });
        var result = await response.Content.ReadFromJsonAsync<TokenResponseDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.NotEqual(original.RefreshToken, result.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_Returns404()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto { RefreshToken = "invalidtoken" });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_AlreadyUsedToken_Returns404()
    {
        var tokenResponse = await _client.PostAsJsonAsync("/api/auth/token",
            new TokenRequestDto { Email = $"test_{Guid.NewGuid()}@test.com" });
        var original = await tokenResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

        await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto { RefreshToken = original!.RefreshToken });

        var response = await _client.PostAsJsonAsync("/api/auth/refresh",
            new RefreshRequestDto { RefreshToken = original.RefreshToken });

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task InvalidateToken_ValidToken_Returns200()
    {
        var tokenResponse = await _client.PostAsJsonAsync("/api/auth/token",
            new TokenRequestDto { Email = $"test_{Guid.NewGuid()}@test.com" });
        var token = await tokenResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

        var response = await _client.PostAsJsonAsync("/api/auth/invalidate",
            new InvalidateRequestDto { AccessToken = token!.AccessToken });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ValidateToken_ValidToken_Returns200()
    {
        var tokenResponse = await _client.PostAsJsonAsync("/api/auth/token",
            new TokenRequestDto { Email = $"test_{Guid.NewGuid()}@test.com" });
        var token = await tokenResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

        var response = await _client.PostAsJsonAsync("/api/auth/validate",
            new ValidateTokenDto { AccessToken = token!.AccessToken });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ValidateToken_InvalidatedToken_Returns401()
    {
        var tokenResponse = await _client.PostAsJsonAsync("/api/auth/token",
            new TokenRequestDto { Email = $"test_{Guid.NewGuid()}@test.com" });
        var token = await tokenResponse.Content.ReadFromJsonAsync<TokenResponseDto>();

        await _client.PostAsJsonAsync("/api/auth/invalidate",
            new InvalidateRequestDto { AccessToken = token!.AccessToken });

        var response = await _client.PostAsJsonAsync("/api/auth/validate",
            new ValidateTokenDto { AccessToken = token.AccessToken });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetPublicKey_Returns200WithPemKey()
    {
        var response = await _client.GetAsync("/api/auth/public-key");
        var key = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("BEGIN PUBLIC KEY", key);
    }

}
