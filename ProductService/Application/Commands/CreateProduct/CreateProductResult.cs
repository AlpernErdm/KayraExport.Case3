namespace ProductService.Application.Commands.CreateProduct;

public class CreateProductResult
{
    public bool IsSuccess { get; private set; }
    
    public string? ErrorMessage { get; private set; }
    
    public Guid? ProductId { get; private set; }
    
    public static CreateProductResult Success(Guid productId) => new()
    {
        IsSuccess = true,
        ProductId = productId
    };
    
    public static CreateProductResult Failure(string errorMessage) => new()
    {
        IsSuccess = false,
        ErrorMessage = errorMessage
    };
}
