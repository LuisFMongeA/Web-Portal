using UserService.Models.DTOs;

namespace UserService.Models.Interfaces;

public interface IUserService
{
    Task CreateUser(CreateUserDto userDto);
    Task<ResponseDto> LoginUser(LoginUserDto loginUserDto);
}
