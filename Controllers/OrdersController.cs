using Microsoft.AspNetCore.Mvc;
using OrdenesAPI.DTOs;
using OrdenesAPI.Services.Interfaces;

namespace OrdenesAPI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de órdenes
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Crea una nueva orden
        /// </summary>
        /// <param name="request">Datos de la orden a crear</param>
        /// <returns>La orden creada con su ID generado</returns>
        /// <response code="201">Orden creada exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var order = await _orderService.CreateOrderAsync(request);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.OrderId }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una orden por su ID
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <returns>Detalles de la orden</returns>
        /// <response code="200">Orden encontrada</response>
        /// <response code="404">Orden no encontrada</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound(new { message = $"Orden con ID {id} no encontrada." });

            return Ok(order);
        }

        /// <summary>
        /// Lista órdenes con filtros y paginación
        /// </summary>
        /// <param name="status">Filtrar por estado</param>
        /// <param name="customerId">Filtrar por cliente</param>
        /// <param name="pageNumber">Número de página (por defecto 1)</param>
        /// <param name="pageSize">Tamaño de página (por defecto 10)</param>
        /// <returns>Lista paginada de órdenes</returns>
        /// <response code="200">Órdenes obtenidas exitosamente</response>
        /// <response code="400">Parámetros inválidos</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<OrderResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetOrders(
            [FromQuery] string? status,
            [FromQuery] Guid? customerId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10
        )
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("pageNumber y pageSize deben ser mayores que cero.");

            var result = await _orderService.GetOrdersAsync(
                status,
                customerId,
                pageNumber,
                pageSize
            );

            return Ok(result);
        }

        /// <summary>
        /// Actualiza el estado de una orden
        /// </summary>
        /// <param name="id">ID de la orden</param>
        /// <param name="request">Nuevo estado</param>
        /// <returns>Orden actualizada</returns>
        /// <response code="200">Estado actualizado correctamente</response>
        /// <response code="400">Estado inválido</response>
        /// <response code="404">Orden no encontrada</response>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(
            Guid id,
            [FromBody] UpdateOrderStatusRequest request
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var updatedOrder = await _orderService.UpdateOrderStatusAsync(
                    id,
                    request.NewStatus
                );
                return Ok(updatedOrder);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Orden con ID {id} no encontrada." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
