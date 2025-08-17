using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Queries.GetProducts;
using Shared.DTOs;
using System.Security.Claims;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IMediator mediator, ILogger<ProductController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ProductListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? category = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null)
    {
        try
        {
            _logger.LogInformation("GetProducts request received: Page={Page}, Size={Size}, Category={Category}, Search={Search}", 
                pageNumber, pageSize, category, searchTerm);
            
            var query = new GetProductsQuery(pageNumber, pageSize, category, searchTerm, minPrice, maxPrice);
            var result = await _mediator.Send(query);
            
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            
            return BadRequest(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting products");
            return StatusCode(500, "An internal server error occurred");
        }
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreateProductResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        try
        {
            _logger.LogInformation("CreateProduct request received for: {ProductName}", request.Name);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("User ID not found in JWT token");
                return Unauthorized("Invalid user token");
            }
            
            var command = new CreateProductCommand(request, userId);
            var result = await _mediator.Send(command);
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("Product created successfully: {ProductId}", result.ProductId);
                return CreatedAtAction(nameof(GetProducts), new { id = result.ProductId }, result);
            }
            
            if (result.ErrorMessage?.Contains("already exists") == true)
            {
                return Conflict(result);
            }
            
            return BadRequest(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating product: {ProductName}", request.Name);
            return StatusCode(500, "An internal server error occurred");
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProduct(Guid id)
    {
        try
        {
            _logger.LogInformation("GetProduct request received for ID: {ProductId}", id);
            
            return NotFound("GetProductById query not implemented yet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting product: {ProductId}", id);
            return StatusCode(500, "An internal server error occurred");
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            _logger.LogInformation("UpdateProduct request received for ID: {ProductId}", id);
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            return NotFound("UpdateProduct command not implemented yet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating product: {ProductId}", id);
            return StatusCode(500, "An internal server error occurred");
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        try
        {
            _logger.LogInformation("DeleteProduct request received for ID: {ProductId}", id);
            
            return NotFound("DeleteProduct command not implemented yet");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting product: {ProductId}", id);
            return StatusCode(500, "An internal server error occurred");
        }
    }
}
