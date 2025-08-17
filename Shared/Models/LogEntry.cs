using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class LogEntry : BaseEntity
{
    [Required]
    [StringLength(100)]
    public string ServiceName { get; set; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Level { get; set; } = string.Empty;
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public string? ContextData { get; set; }
    
    public string? Exception { get; set; }
    
    public Guid? UserId { get; set; }
    
    [StringLength(100)]
    public string? CorrelationId { get; set; }
    
    [StringLength(45)]
    public string? IpAddress { get; set; }
    
    [StringLength(500)]
    public string? UserAgent { get; set; }
}
