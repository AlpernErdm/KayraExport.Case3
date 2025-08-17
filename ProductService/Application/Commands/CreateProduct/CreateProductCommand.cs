using MediatR;
using Shared.DTOs;

namespace ProductService.Application.Commands.CreateProduct;

public class CreateProductCommand : IRequest<CreateProductResult>
{
    public CreateProductRequest Request { get; }
    
    public Guid CreatedByUserId { get; }

    public CreateProductCommand(CreateProductRequest request, Guid createdByUserId)
    {
        Request = request;
        CreatedByUserId = createdByUserId;
    }
}
