using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public record CreateLogRequest
{
    [Required]
    [StringLength(100)]
    public string ServiceName { get; init; } = string.Empty;
    
    [Required]
    [StringLength(20)]
    public string Level { get; init; } = string.Empty;
    
    [Required]
    public string Message { get; init; } = string.Empty;
    
    public string? ContextData { get; init; }
    
    public string? Exception { get; init; }
    
    public Guid? UserId { get; init; }
    
    [StringLength(100)]
    public string? CorrelationId { get; init; }
    
    [StringLength(45)]
    public string? IpAddress { get; init; }
    
    [StringLength(500)]
    public string? UserAgent { get; init; }
}

public record GetLogsRequest
{
    public int PageNumber { get; init; } = 1;
    
    public int PageSize { get; init; } = 50;
    
    public string? ServiceName { get; init; }
    
    public string? Level { get; init; }
    
    public string? SearchTerm { get; init; }
    
    public DateTime? FromDate { get; init; }
    
    public DateTime? ToDate { get; init; }
    
    public string? CorrelationId { get; init; }
}

public record LogEntryResponse
{
    public Guid Id { get; init; }
    
    public string ServiceName { get; init; } = string.Empty;
    
    public string Level { get; init; } = string.Empty;
    
    public string Message { get; init; } = string.Empty;
    
    public string? ContextData { get; init; }
    
    public string? Exception { get; init; }
    
    public Guid? UserId { get; init; }
    
    public string? CorrelationId { get; init; }
    
    public string? IpAddress { get; init; }
    
    public string? UserAgent { get; init; }
    
    public DateTime CreatedAt { get; init; }
}

public record LogListResponse
{
    public List<LogEntryResponse> LogEntries { get; init; } = new();
    
    public int TotalCount { get; init; }
    
    public int PageNumber { get; init; }
    
    public int PageSize { get; init; }
    
    public int TotalPages { get; init; }
}

public record LogLevelCount
{
    public string Level { get; init; } = string.Empty;
    
    public int Count { get; init; }
}

public record LogStatisticsResponse
{
    public string ServiceName { get; init; } = string.Empty;
    
    public int TotalLogs { get; init; }
    
    public double ErrorRate { get; init; }
    
    public List<LogLevelCount> LevelCounts { get; init; } = new();
    
    public DateTime? FromDate { get; init; }
    
    public DateTime? ToDate { get; init; }
}
