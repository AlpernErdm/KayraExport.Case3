using System.ComponentModel.DataAnnotations;

namespace Shared.Models;

public class Product : BaseEntity
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(1000)]
    public string? Description { get; set; }
    
    [Required]
    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    [StringLength(100)]
    public string? Category { get; set; }
    
    [StringLength(50)]
    public string? SKU { get; set; }
}
