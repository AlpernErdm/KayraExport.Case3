using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public record LoginRequest
{
    [Required]
    public string Username { get; init; } = string.Empty;
    
    [Required]
    public string Password { get; init; } = string.Empty;
}

public record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    
    public string RefreshToken { get; init; } = string.Empty;
    
    public DateTime ExpiresAt { get; init; }
    
    public UserInfo User { get; init; } = new();
}

public record UserInfo
{
    public Guid Id { get; init; }
    
    public string Username { get; init; } = string.Empty;
    
    public string Email { get; init; } = string.Empty;
    
    public List<string> Roles { get; init; } = new();
}

public record RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; init; } = string.Empty;
}

public record RegisterRequest
{
    [Required]
    [StringLength(50)]
    public string Username { get; init; } = string.Empty;
    
    [Required]
    [EmailAddress]
    public string Email { get; init; } = string.Empty;
    
    [Required]
    [MinLength(6)]
    public string Password { get; init; } = string.Empty;
    
    public string? FirstName { get; init; }
    
    public string? LastName { get; init; }
}
