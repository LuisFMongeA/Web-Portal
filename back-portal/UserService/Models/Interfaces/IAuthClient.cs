using UserService.Models.DTOs;

namespace UserService.Models.Interfaces;

public interface IAuthClient
{
    Task<ResponseDto> GetTokenAsync(string email);
}
