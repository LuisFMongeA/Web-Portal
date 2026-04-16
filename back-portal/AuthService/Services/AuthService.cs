using AuthService.Models.DTOs;
using AuthService.Models.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthService.Services;

public class AuthService : IAuthService
{
    private readonly RSA _rsa;
    private readonly IMemoryCache _memoryCache;

    public AuthService(IMemoryCache memoryCache) 
    {
        _memoryCache = memoryCache;
        _rsa = RSA.Create(2048);

    }
    public TokenResponseDto GenerateToken(string email)
    {
        if (!IsValidEmail(email)) throw new ArgumentException("Email address incorrect");

        var securityKey = new RsaSecurityKey(_rsa);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: "AuthService",
            audience: "TodoPortal",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Guid.NewGuid().ToString();

        _memoryCache.Set($"refresh_{refreshToken}", email, DateTimeOffset.UtcNow.AddSeconds(3600));

        return new TokenResponseDto
        {
            AccessToken = tokenString,
            RefreshToken = refreshToken,
            ExpiresIn = 900
        };
    }

    public void InvalidateToken(string accessToken) {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        var expiration = jwtToken.ValidTo;
        var remainingTime = expiration - DateTime.UtcNow;

        if (remainingTime > TimeSpan.Zero) {

            _memoryCache.Set($"blacklist_{accessToken}", true, remainingTime);
        }
        
    } 

    public TokenResponseDto RefreshToken(string token)
    {
        if (!_memoryCache.TryGetValue<string>($"refresh_{token}", out var email))
            throw new ArgumentException("Token not found");
        _memoryCache.Remove($"refresh_{token}");
        return GenerateToken(email!);
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public string GetPublicKey()
    {
        return _rsa.ExportSubjectPublicKeyInfoPem();
    }

    public bool ValidateToken(string accessToken)
    {
        return _memoryCache.Get($"blacklist_{accessToken}") == null && CryptoValidation(accessToken);
    }

    private bool CryptoValidation(string accessToken) 
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(_rsa),
            ValidateIssuer = true,
            ValidIssuer = "AuthService",
            ValidateAudience = true,
            ValidAudience = "TodoPortal",
            ValidateLifetime = true
        };

        try
        {
            tokenHandler.ValidateToken(accessToken, validationParameters, out _);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
