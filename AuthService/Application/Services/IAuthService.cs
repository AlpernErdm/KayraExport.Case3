using Shared.DTOs;
using Shared.Models;

namespace AuthService.Application.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request);
    
    Task<AuthResult<LoginResponse>> LoginAsync(LoginRequest request);
    
    Task<AuthResult<string>> RefreshTokenAsync(RefreshTokenRequest request);
}
