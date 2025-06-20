using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdenesAPI.Data;
using OrdenesAPI.DTOs;
using OrdenesAPI.Models;

namespace OrdenesAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var orderItems = new List<OrderItem>();
            decimal totalAmount = 0;

            foreach (var item in request.OrderItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                    return BadRequest($"Producto con ID {item.ProductId} no existe.");

                if (product.StockQuantity < item.Quantity)
                    return BadRequest($"Stock insuficiente para el producto {product.Name}.");

                product.StockQuantity -= item.Quantity;

                var unitPrice = product.CurrentUnitPrice;
                var subtotal = unitPrice * item.Quantity;
                totalAmount += subtotal;

                orderItems.Add(
                    new OrderItem
                    {
                        ProductId = product.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = unitPrice,
                        Subtotal = subtotal,
                    }
                );
            }

            var newOrder = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                ShippingAddress = request.ShippingAddress,
                BillingAddress = request.BillingAddress,
                OrderStatus = "Pending",
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                OrderItems = orderItems,
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderById), new { id = newOrder.OrderId }, newOrder);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            var order = await _context
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }
    }
}
