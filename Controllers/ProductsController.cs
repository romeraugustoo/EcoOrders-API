using Microsoft.AspNetCore.Mvc;
using OrdenesAPI.DTOs;
using OrdenesAPI.Services.Interfaces;

namespace OrdenesAPI.Controllers
{
    /// <summary>
    /// Controlador para la gestión de productos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <param name="request">Datos del producto a crear</param>
        /// <returns>El producto creado con su ID generado</returns>
        /// <response code="201">Producto creado exitosamente</response>
        /// <response code="400">Datos de entrada inválidos</response>
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = await _productService.CreateProductAsync(request);
            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        /// <summary>
        /// Obtiene un producto por su ID
        /// </summary>
        /// <param name="id">ID del producto</param>
        /// <returns>Detalles del producto</returns>
        /// <response code="200">Producto encontrado</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Lista productos con paginación y filtros opcionales
        /// </summary>
        /// <param name="pageNumber">Número de página (predeterminado 1)</param>
        /// <param name="pageSize">Tamaño de página (predeterminado 10)</param>
        /// <param name="searchTerm">Término para buscar en nombre o descripción</param>
        /// <param name="minPrice">Precio mínimo</param>
        /// <param name="maxPrice">Precio máximo</param>
        /// <returns>Lista paginada de productos</returns>
        /// <response code="200">Lista de productos obtenida exitosamente</response>
        /// <response code="400">Parámetros de paginación inválidos</response>
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponse<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null
        )
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("pageNumber y pageSize deben ser mayores que cero.");

            var result = await _productService.GetProductsAsync(
                pageNumber,
                pageSize,
                searchTerm,
                minPrice,
                maxPrice
            );

            return Ok(result);
        }

        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <param name="id">ID del producto a actualizar</param>
        /// <param name="request">Campos a actualizar</param>
        /// <returns>El producto actualizado</returns>
        /// <response code="200">Producto actualizado exitosamente</response>
        /// <response code="400">Solicitud inválida</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProduct(
            Guid id,
            [FromBody] UpdateProductRequest request
        )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var product = await _productService.UpdateProductAsync(id, request);
                return Ok(product);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}