using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;
using Shared.Models;
using AuthService.Application.Services;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthResult), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            _logger.LogInformation("Registration request received for user: {Username}", request.Username);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(AuthResult.Failure("Invalid request data"));
            }
            
            var result = await _authService.RegisterAsync(request);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("User {Username} registered successfully", request.Username);
                return Ok(result);
            }
            
            if (result.ErrorMessage?.Contains("already exists") == true)
            {
                return Conflict(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during user registration for {Username}", request.Username);
            return StatusCode(500, AuthResult.Failure("An internal server error occurred"));
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResult<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResult<LoginResponse>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthResult<LoginResponse>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            _logger.LogInformation("Login request received for user: {Username}", request.Username);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(AuthResult<LoginResponse>.Failure("Invalid request data"));
            }
            
            var result = await _authService.LoginAsync(request);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("User {Username} logged in successfully", request.Username);
                return Ok(result);
            }
            
            if (result.ErrorMessage?.Contains("Invalid username or password") == true)
            {
                return BadRequest(result);
            }
            
            if (result.ErrorMessage?.Contains("deactivated") == true)
            {
                return Unauthorized(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for {Username}", request.Username);
            return StatusCode(500, AuthResult<LoginResponse>.Failure("An internal server error occurred"));
        }
    }

    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            _logger.LogInformation("Token refresh request received");
            
            if (!ModelState.IsValid)
            {
                return BadRequest(AuthResult<string>.Failure("Invalid request data"));
            }
            
            var result = await _authService.RefreshTokenAsync(request);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Token refreshed successfully");
                return Ok(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during token refresh");
            return StatusCode(500, AuthResult<string>.Failure("An internal server error occurred"));
        }
    }
}
