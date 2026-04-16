using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Models.Interfaces;

namespace UserService.Services;

public class UserService(IUserRepository userRepository, IAuthClient authClient) : IUserService
{
    public Task CreateUser(CreateUserDto userDto)
    {
        User user = new User { Email = userDto.Email, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow };
         return userRepository.AddAsync(user);
    }

    public async Task<ResponseDto> LoginUser(LoginUserDto loginUserDto)
    {
        User? user = await userRepository.FindAsync(loginUserDto.Email);
        if (user == null) 
            throw new ArgumentException("User not found");
       
        return await authClient.GetTokenAsync(user.Email);
    }
}
