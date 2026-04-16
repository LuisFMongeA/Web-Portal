using UserService.Models.Entities;

namespace UserService.Models.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> FindAsync(string email);
}
