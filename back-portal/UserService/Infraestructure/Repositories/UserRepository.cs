using UserService.Models.Entities;
using UserService.Models.Interfaces;

namespace UserService.Infraestructure.Repositories;

public class UserRepository : IUserRepository
{
    private Dictionary<string, User> _userDictionary = new Dictionary<string, User>();

    public Task AddAsync(User user)
    {
        if (_userDictionary.ContainsKey(user.Email))
            throw new ArgumentException("Email already exists");

        _userDictionary.TryAdd(user.Email, user);
        return Task.CompletedTask;
        
    }

    public Task<User?> FindAsync(string email) 
    {
        _userDictionary.TryGetValue(email, out var user);
        return Task.FromResult(user);
    } 

}