using System.Security.Claims;
using Shared.Models;

namespace AuthService.Infrastructure.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    
    string GenerateRefreshToken();
    
    ClaimsPrincipal? ValidateToken(string token);
}
