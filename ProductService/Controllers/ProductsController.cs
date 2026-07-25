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

        /// <summary>Get all products with pagination and optional name search.</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<ProductDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var result = await _productService.GetAllAsync(pageNumber, pageSize, search);
            return Ok(ApiResponse<PagedResultDto<ProductDto>>.Ok(result));
        }

        /// <summary>Get a single product by id.</summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _productService.GetByIdAsync(id);
            return Ok(ApiResponse<ProductDto>.Ok(result));
        }

        /// <summary>Create a new product.</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.ProductId },
                ApiResponse<ProductDto>.Ok(result, "Product created successfully."));
        }

        /// <summary>Update an existing product.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(id, dto);
            return Ok(ApiResponse<ProductDto>.Ok(result, "Product updated successfully."));
        }

        /// <summary>Delete (deactivate) a product.</summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _productService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(new { }, "Product deleted successfully."));
        }

        /// <summary>
        /// Internal endpoint used ONLY by the Order Service to validate whether a product
        /// has enough stock for a given quantity. Order Service must never touch the Product
        /// database directly - this API is the sole contract between the two services.
        /// </summary>
        [HttpGet("{id:guid}/check-stock")]
        [ProducesResponseType(typeof(ApiResponse<StockCheckResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckStock(Guid id, [FromQuery] int quantity = 1)
        {
            var result = await _productService.CheckStockAsync(id, quantity);
            return Ok(ApiResponse<StockCheckResponseDto>.Ok(result));
        }

        /// <summary>
        /// Internal endpoint used ONLY by the Order Service to atomically reduce stock after
        /// a successful order. The reduction fails (409) if requested quantity exceeds stock.
        /// </summary>
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
