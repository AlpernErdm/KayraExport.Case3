using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Services;
using Shared.Events;
using Shared.Interfaces;
using Shared.Models;

namespace ProductService.Application.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResult>
{
    private readonly ProductDbContext _context;
    private readonly IRedisCacheService _cacheService;
    private readonly IEventBus _eventBus;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        ProductDbContext context,
        IRedisCacheService cacheService,
        IEventBus eventBus,
        ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating product: {ProductName}", request.Request.Name);
            
            if (!string.IsNullOrEmpty(request.Request.SKU))
            {
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.SKU == request.Request.SKU, cancellationToken);
                
                if (existingProduct != null)
                {
                    _logger.LogWarning("Product creation failed: SKU {SKU} already exists", request.Request.SKU);
                    return CreateProductResult.Failure($"SKU '{request.Request.SKU}' already exists");
                }
            }
            
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Request.Name,
                Description = request.Request.Description,
                Price = request.Request.Price,
                StockQuantity = request.Request.StockQuantity,
                Category = request.Request.Category,
                SKU = request.Request.SKU,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);
            
            await _cacheService.RemovePatternAsync("products:*");
            
            var productCreatedEvent = new ProductCreatedEvent
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Category = product.Category,
                Price = product.Price,
                CreatedByUserId = request.CreatedByUserId,
                CorrelationId = Guid.NewGuid().ToString()
            };
            
            await _eventBus.PublishAsync(productCreatedEvent);
            
            _logger.LogInformation("Product created successfully: {ProductId} - {ProductName}", 
                product.Id, product.Name);
            
            return CreateProductResult.Success(product.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product: {ProductName}", request.Request.Name);
            return CreateProductResult.Failure("An error occurred while creating the product");
        }
    }
}
