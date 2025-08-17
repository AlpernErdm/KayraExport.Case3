using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Services;
using Shared.DTOs;
using System.Text.Json;

namespace ProductService.Application.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, GetProductsResult>
{
    private readonly ProductDbContext _context;
    private readonly IRedisCacheService _cacheService;
    private readonly ILogger<GetProductsQueryHandler> _logger;

    public GetProductsQueryHandler(
        ProductDbContext context,
        IRedisCacheService cacheService,
        ILogger<GetProductsQueryHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<GetProductsResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting products with filters: Page={Page}, Size={Size}, Category={Category}, Search={Search}", 
                request.PageNumber, request.PageSize, request.Category, request.SearchTerm);
            
            var cacheKey = GenerateCacheKey(request);
            var cachedResult = await _cacheService.GetAsync<ProductListResponse>(cacheKey);
            
            if (cachedResult != null)
            {
                _logger.LogDebug("Products retrieved from cache for key: {CacheKey}", cacheKey);
                return GetProductsResult.Success(cachedResult);
            }
            
            var query = _context.Products.AsNoTracking().Where(p => p.IsActive);
            
            if (!string.IsNullOrEmpty(request.Category))
            {
                query = query.Where(p => p.Category == request.Category);
            }
            
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(request.SearchTerm) || 
                                        (p.Description != null && p.Description.Contains(request.SearchTerm)));
            }
            
            if (request.MinPrice.HasValue)
            {
                query = query.Where(p => p.Price >= request.MinPrice.Value);
            }
            
            if (request.MaxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= request.MaxPrice.Value);
            }
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            var products = await query
                .OrderBy(p => p.Name)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new ProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    Category = p.Category,
                    SKU = p.SKU,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt
                })
                .ToListAsync(cancellationToken);
            
            var result = new ProductListResponse
            {
                Products = products,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };
            
            await _cacheService.SetAsync(cacheKey, result, 30);
            
            _logger.LogInformation("Products retrieved successfully: {Count} products, Total: {TotalCount}", 
                products.Count, totalCount);
            
            return GetProductsResult.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting products");
            return GetProductsResult.Failure("An error occurred while retrieving products");
        }
    }
    
    private static string GenerateCacheKey(GetProductsQuery query)
    {
        var keyParts = new List<string>
        {
            "products",
            $"page:{query.PageNumber}",
            $"size:{query.PageSize}"
        };
        
        if (!string.IsNullOrEmpty(query.Category))
            keyParts.Add($"category:{query.Category}");
        
        if (!string.IsNullOrEmpty(query.SearchTerm))
            keyParts.Add($"search:{query.SearchTerm}");
        
        if (query.MinPrice.HasValue)
            keyParts.Add($"minprice:{query.MinPrice}");
        
        if (query.MaxPrice.HasValue)
            keyParts.Add($"maxprice:{query.MaxPrice}");
        
        return string.Join(":", keyParts);
    }
}
