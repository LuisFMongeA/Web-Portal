using System.Net;
using TodoService.Models.Interfaces;

namespace TodoService.Infraestructure.Clients;

public class AuthClient : IAuthClient
{
    private HttpClient _httpClient;
    private IConfiguration _configuration;
    public AuthClient(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient("AuthService");
        _configuration = configuration;
    }

    public async Task<bool> IsTokenValid(string accessToken)
    {
        try
        {
            string endpoint = _configuration["Auth:ValidateToken"] ?? throw new Exception("Auth:ValidateToken not found");
            var response = await _httpClient.PostAsJsonAsync(endpoint, new { AccessToken = accessToken });
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }
}
