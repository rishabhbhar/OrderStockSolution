using Microsoft.AspNetCore.Mvc;
using ProductService.Common;
using ProductService.DTOs;
using ProductService.Services;

namespace ProductService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<ProductDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize, search);
            return Ok(ApiResponse<PagedResultDto<ProductDto>>.Ok(result));
        }

        
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(ApiResponse<ProductDto>.Ok(result));
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId },
                ApiResponse<ProductDto>.Ok(result, "Product created successfully."));
        }

      
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(id, dto);
            return Ok(ApiResponse<ProductDto>.Ok(result, "Product updated successfully."));
        }

        
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Product deleted successfully."));
        }

       
        [HttpGet("{id:guid}/check-stock")]
        [ProducesResponseType(typeof(ApiResponse<StockCheckResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckStock(Guid id, [FromQuery] int quantity = 1)
        {
            var result = await _productService.CheckStockAsync(id, quantity);
            return Ok(ApiResponse<StockCheckResponseDto>.Ok(result));
        }

        
        [HttpPost("{id:guid}/reduce-stock")]
        [ProducesResponseType(typeof(ApiResponse<ReduceStockResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReduceStock(Guid id, [FromBody] ReduceStockRequestDto dto)
        {
            var result = await _productService.ReduceStockAsync(id, dto.Quantity);
            return Ok(ApiResponse<ReduceStockResponseDto>.Ok(result, "Stock reduced successfully."));
        }
    }
}
