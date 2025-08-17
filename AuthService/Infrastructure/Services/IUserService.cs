using Shared.Models;

namespace AuthService.Infrastructure.Services;

public interface IUserService
{
    Task<bool> CreateUserAsync(User user, string password);
    
    Task<User?> FindByUsernameAsync(string username);
    
    Task<User?> FindByEmailAsync(string email);
    
    Task<bool> CheckPasswordAsync(User user, string password);
    
    Task<List<string>> GetUserRolesAsync(User user);
}
