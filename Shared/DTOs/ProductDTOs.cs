using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs;

public record CreateProductRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; init; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; init; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; init; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; init; }
    
    [StringLength(100)]
    public string? Category { get; init; }
    
    [StringLength(50)]
    public string? SKU { get; init; }
}

public record UpdateProductRequest
{
    [StringLength(200)]
    public string? Name { get; init; }
    
    [StringLength(1000)]
    public string? Description { get; init; }
    
    [Range(0, double.MaxValue)]
    public decimal? Price { get; init; }
    
    [Range(0, int.MaxValue)]
    public int? StockQuantity { get; init; }
    
    [StringLength(100)]
    public string? Category { get; init; }
    
    [StringLength(50)]
    public string? SKU { get; init; }
}

public record ProductResponse
{
    public Guid Id { get; init; }
    
    public string Name { get; init; } = string.Empty;
    
    public string? Description { get; init; }
    
    public decimal Price { get; init; }
    
    public int StockQuantity { get; init; }
    
    public string? Category { get; init; }
    
    public string? SKU { get; init; }
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime? UpdatedAt { get; init; }
}

public record ProductListResponse
{
    public List<ProductResponse> Products { get; init; } = new();
    
    public int TotalCount { get; init; }
    
    public int PageNumber { get; init; }
    
    public int PageSize { get; init; }
    
    public int TotalPages { get; init; }
}
