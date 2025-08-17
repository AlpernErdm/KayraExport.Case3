using MediatR;
using Shared.DTOs;

namespace ProductService.Application.Queries.GetProducts;

public class GetProductsQuery : IRequest<GetProductsResult>
{
    public int PageNumber { get; }
    
    public int PageSize { get; }
    
    public string? Category { get; }
    
    public string? SearchTerm { get; }
    
    public decimal? MinPrice { get; }
    
    public decimal? MaxPrice { get; }

    public GetProductsQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? category = null,
        string? searchTerm = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        PageNumber = Math.Max(1, pageNumber);
        PageSize = Math.Max(1, Math.Min(100, pageSize));
        Category = category;
        SearchTerm = searchTerm;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
    }
}
