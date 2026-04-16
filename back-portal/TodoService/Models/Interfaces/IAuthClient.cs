namespace TodoService.Models.Interfaces;

public interface IAuthClient
{
    Task<bool> IsTokenValid(string token);
}
