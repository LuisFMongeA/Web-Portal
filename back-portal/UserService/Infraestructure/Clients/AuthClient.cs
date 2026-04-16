using UserService.Models.DTOs;
using UserService.Models.Interfaces;

namespace UserService.Infraestructure.Clients;

public class AuthClient : IAuthClient
{
    private HttpClient _httpClient;
    private IConfiguration _configuration;
    public AuthClient(IHttpClientFactory httpClientFactory, IConfiguration configuration) 
    {
        _httpClient = httpClientFactory.CreateClient("AuthService");
        _configuration = configuration;
    }

    public async Task<ResponseDto> GetTokenAsync(string email)
    {
        string endpoint = _configuration["Auth:GetToken"] ?? throw new Exception("Auth:GetToken not found");
        var response = await _httpClient.PostAsJsonAsync(endpoint, new {Email = email});
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ResponseDto>() ?? throw new Exception("Invalid response from AuthService"); ;
        result.Email = email;
        return result;
    }
}
