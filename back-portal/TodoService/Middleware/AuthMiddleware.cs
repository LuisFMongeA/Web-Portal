
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TodoService.Models.Interfaces;

namespace TodoService.Middleware;

public class AuthMiddleware(IAuthClient authClient) : IMiddleware
{

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var authHeader = context.Request.Headers.Authorization.ToString();
        if (string.IsNullOrEmpty(authHeader))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var token = authHeader.Replace("Bearer ", "");

        

        var response = await authClient.IsTokenValid(token);
        if (!response)
        {
            context.Response.StatusCode = 401;
            return;
        }
        
        try
        {
            ExtractClaims(context, token);
            await next(context);
        }
        catch
        {
            context.Response.StatusCode = 400;
            return;
        }
    }
    private void ExtractClaims(HttpContext context, string token) 
    {
        var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var email = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email!) });
        context.User = new ClaimsPrincipal(identity);
    }
}
