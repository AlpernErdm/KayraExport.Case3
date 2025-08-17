using Microsoft.Extensions.Logging;
using Shared.DTOs;
using Shared.Models;
using AuthService.Infrastructure.Services;

namespace AuthService.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserService userService,
        IJwtService jwtService,
        ILogger<AuthService> logger)
    {
        _userService = userService;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Attempting to register user: {Username}", request.Username);
            
            var existingUser = await _userService.FindByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                _logger.LogWarning("User registration failed: Username {Username} already exists", request.Username);
                return AuthResult.Failure("Username already exists");
            }
            
            existingUser = await _userService.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("User registration failed: Email {Email} already exists", request.Email);
                return AuthResult.Failure("Email already exists");
            }
            
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                Roles = new List<string> { "User" }
            };
            
            var result = await _userService.CreateUserAsync(user, request.Password);
            if (!result)
            {
                _logger.LogError("User creation failed");
                return AuthResult.Failure("Failed to create user");
            }
            
            _logger.LogInformation("User {Username} registered successfully", request.Username);
            return AuthResult.Success("User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during user registration for {Username}", request.Username);
            return AuthResult.Failure("An error occurred during registration");
        }
    }

    public async Task<AuthResult<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {Username}", request.Username);
            
            var user = await _userService.FindByUsernameAsync(request.Username) 
                      ?? await _userService.FindByEmailAsync(request.Username);
            
            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Username} not found", request.Username);
                return AuthResult<LoginResponse>.Failure("Invalid username or password");
            }
            
            if (!user.IsActive)
            {
                _logger.LogWarning("Login failed: User {Username} account is deactivated", request.Username);
                return AuthResult<LoginResponse>.Failure("Account is deactivated");
            }
            
            if (!await _userService.CheckPasswordAsync(user, request.Password))
            {
                _logger.LogWarning("Login failed: Invalid password for user {Username}", request.Username);
                return AuthResult<LoginResponse>.Failure("Invalid username or password");
            }
            
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();
            
            var roles = await _userService.GetUserRolesAsync(user);
            user.Roles = roles;
            
            var response = new LoginResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                User = new UserInfo
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Roles = user.Roles
                }
            };
            
            _logger.LogInformation("User {Username} logged in successfully", request.Username);
            return AuthResult<LoginResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for {Username}", request.Username);
            return AuthResult<LoginResponse>.Failure("An error occurred during login");
        }
    }

    public async Task<AuthResult<string>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            _logger.LogInformation("Token refresh attempt");
            
            _logger.LogWarning("Token refresh not implemented yet");
            return AuthResult<string>.Failure("Token refresh not implemented yet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during token refresh");
            return AuthResult<string>.Failure("An error occurred during token refresh");
        }
    }
}
