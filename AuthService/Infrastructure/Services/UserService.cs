using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AuthService.Infrastructure.Data;
using Shared.Models;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly AuthDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(AuthDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> CreateUserAsync(User user, string password)
    {
        try
        {
            user.PasswordHash = HashPassword(password);
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User created successfully: {Username}", user.Username);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user: {Username}", user.Username);
            return false;
        }
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        var hashedPassword = HashPassword(password);
        return user.PasswordHash == hashedPassword;
    }

    public async Task<List<string>> GetUserRolesAsync(User user)
    {
        return user.Roles ?? new List<string>();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}
