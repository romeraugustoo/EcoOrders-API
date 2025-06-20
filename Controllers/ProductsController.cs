using Microsoft.AspNetCore.Mvc;
using OrdenesAPI.Data;
using OrdenesAPI.DTOs;
using OrdenesAPI.Models;

namespace OrdenesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                SKU = request.SKU,
                InternalCode = request.InternalCode,
                Name = request.Name,
                Description = request.Description,
                CurrentUnitPrice = request.CurrentUnitPrice,
                StockQuantity = request.StockQuantity,
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.ProductId }, product);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }
    }
}
