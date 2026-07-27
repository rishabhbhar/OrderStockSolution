using Microsoft.AspNetCore.Mvc;
using OrderService.Common;
using OrderService.DTOs;
using OrderService.Entities;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

       
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<OrderDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderStatus? status = null)
        {
            var result = await _orderService.GetAllAsync(pageNumber, pageSize, status);
            return Ok(ApiResponse<PagedResultDto<OrderDto>>.Ok(result));
        }

        
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _orderService.GetByIdAsync(id);
            return Ok(ApiResponse<OrderDto>.Ok(result));
        }

      
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<OrderDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var result = await _orderService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.OrderId },
                ApiResponse<OrderDto>.Ok(result, "Order created successfully."));
        }
    }
}
