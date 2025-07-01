using Microsoft.EntityFrameworkCore;
using OrdenesAPI.Data;
using OrdenesAPI.DTOs;
using OrdenesAPI.Models;
using OrdenesAPI.Services.Interfaces;

namespace OrdenesAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            // Obtener los productos que coincidan con los IDs de los items
            var productIds = request.Items.Select(i => i.ProductId).ToList();
            var products = await _context
                .Products.Where(p => productIds.Contains(p.ProductId))
                .ToDictionaryAsync(p => p.ProductId);

            // Validar que todos los productos existen y hay stock suficiente
            foreach (var item in request.Items)
            {
                if (!products.ContainsKey(item.ProductId))
                    throw new ArgumentException($"Producto con ID {item.ProductId} no encontrado.");

                var product = products[item.ProductId];

                if (product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException(
                        $"Stock insuficiente para el producto {product.Name}."
                    );
            }

            // Crear la orden
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = "Pending",
                ShippingAddress = request.ShippingAddress,
                BillingAddress = request.BillingAddress,
                Notes = request.Notes,
                OrderItems = request
                    .Items.Select(item =>
                    {
                        var prod = products[item.ProductId];
                        return new OrderItem
                        {
                            OrderItemId = Guid.NewGuid(),
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = prod.CurrentUnitPrice,
                            Subtotal = item.Quantity * prod.CurrentUnitPrice,
                        };
                    })
                    .ToList(),
            };

            // Calcular el total de la orden
            order.TotalAmount = order.OrderItems.Sum(oi => oi.Subtotal);

            // Actualizar stock de productos
            foreach (var item in order.OrderItems)
            {
                var product = products[item.ProductId];
                product.StockQuantity -= item.Quantity;
                _context.Products.Update(product);
            }

            // Guardar la orden y los cambios de stock
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Preparar la respuesta
            return new OrderResponse
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                Notes = order.Notes,
                Items = order
                    .OrderItems.Select(oi => new OrderItemResponse
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = products[oi.ProductId].Name,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal,
                    })
                    .ToList(),
            };
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(Guid orderId)
        {
            var order = await _context
                .Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return null;

            return new OrderResponse
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                Notes = order.Notes,
                Items = order
                    .OrderItems.Select(oi => new OrderItemResponse
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? "(Producto no disponible)",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal,
                    })
                    .ToList(),
            };
        }

        public async Task<PaginatedResponse<OrderResponse>> GetOrdersAsync(
            string? status,
            Guid? customerId,
            int pageNumber,
            int pageSize
        )
        {
            // Validar paginación
            if (pageNumber <= 0 || pageSize <= 0)
                throw new ArgumentException(
                    "Los parámetros 'pageNumber' y 'pageSize' deben ser mayores que cero."
                );

            // Validar estado si viene
            var allowedStatuses = new[]
            {
                "Pending",
                "Processing",
                "Shipped",
                "Delivered",
                "Cancelled",
            };
            if (!string.IsNullOrEmpty(status) && !allowedStatuses.Contains(status))
                throw new ArgumentException(
                    $"El estado '{status}' no es válido. Los valores permitidos son: {string.Join(", ", allowedStatuses)}"
                );

            // Obtener todas las órdenes
            var query = _context.Orders.AsQueryable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(o => o.OrderStatus == status);

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            var totalItems = await query.CountAsync();

            // Paginación
            var orders = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            // Mapear al DTO de respuesta
            var orderResponses = orders
                .Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    OrderStatus = o.OrderStatus,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                })
                .ToList();

            return new PaginatedResponse<OrderResponse>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                Items = orderResponses,
            };
        }

        public async Task<OrderResponse> UpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException("Orden no encontrada.");

            // Lista de estados válidos
            var allowedStatuses = new[]
            {
                "Pending",
                "Processing",
                "Shipped",
                "Delivered",
                "Cancelled",
            };
            if (!allowedStatuses.Contains(newStatus))
                throw new ArgumentException($"Estado '{newStatus}' no es válido.");

            // Validaciones de transición de estado
            var validTransitions = new Dictionary<string, List<string>>
            {
                ["Pending"] = new List<string> { "Processing", "Cancelled" },
                ["Processing"] = new List<string> { "Shipped", "Cancelled" },
                ["Shipped"] = new List<string> { "Delivered" },
                ["Delivered"] = new List<string>(), // No se puede cambiar más
                ["Cancelled"] = new List<string>(), // No se puede cambiar más
            };

            var currentStatus = order.OrderStatus;
            if (
                !validTransitions.ContainsKey(currentStatus)
                || !validTransitions[currentStatus].Contains(newStatus)
            )
                throw new ArgumentException(
                    $"Transición inválida de '{currentStatus}' a '{newStatus}'."
                );

            // Actualizar estado
            order.OrderStatus = newStatus;
            await _context.SaveChangesAsync();

            return new OrderResponse
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                BillingAddress = order.BillingAddress,
                Notes = order.Notes,
                Items = order
                    .OrderItems.Select(oi => new OrderItemResponse
                    {
                        OrderItemId = oi.OrderItemId,
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? "(eliminado)",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                        Subtotal = oi.Subtotal,
                    })
                    .ToList(),
            };
        }
    }
}
