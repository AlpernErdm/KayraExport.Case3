using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class User : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    [StringLength(50)]
    public string? FirstName { get; set; }
    
    [StringLength(50)]
    public string? LastName { get; set; }
    
    public List<string> Roles { get; set; } = new();
    
    public bool IsEmailConfirmed { get; set; }
    
    public bool IsLocked { get; set; }
}
