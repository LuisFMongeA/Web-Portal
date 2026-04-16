using AuthService.Models.DTOs;

namespace AuthService.Models.Interfaces;

public interface IAuthService
{
    TokenResponseDto GenerateToken(string email);
    TokenResponseDto RefreshToken(string token);
    void InvalidateToken(string token);
    bool ValidateToken(string token);
    string GetPublicKey();
}
