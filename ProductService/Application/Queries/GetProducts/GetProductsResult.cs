using Shared.DTOs;

namespace ProductService.Application.Queries.GetProducts;

public class GetProductsResult
{
    public bool IsSuccess { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public ProductListResponse? Data { get; private set; }
    
    public static GetProductsResult Success(ProductListResponse data) => new()
    {
        IsSuccess = true,
        Data = data
    };
    
    public static GetProductsResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}
