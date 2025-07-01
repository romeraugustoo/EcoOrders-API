using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrdenesAPI.Models;

namespace OrdenesAPI.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // 1. Productos iniciales (del README)
            if (!context.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                        SKU = "VERD-001",
                        InternalCode = "LTG-001",
                        Name = "Lechuga",
                        Description = "Lechuga fresca hidropónica",
                        CurrentUnitPrice = 50.00m,
                        StockQuantity = 100,
                    },
                    new Product
                    {
                        ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                        SKU = "VERD-002",
                        InternalCode = "TMT-001",
                        Name = "Tomate",
                        Description = "Tomate perita orgánico",
                        CurrentUnitPrice = 60.00m,
                        StockQuantity = 80,
                    },
                    new Product
                    {
                        ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa8"),
                        SKU = "VERD-003",
                        InternalCode = "ZNH-001",
                        Name = "Zanahoria",
                        Description = "Zanahoria fresca de cosecha local",
                        CurrentUnitPrice = 40.00m,
                        StockQuantity = 120,
                    },
                    new Product
                    {
                        ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa9"),
                        SKU = "FRUT-001",
                        InternalCode = "NAR-001",
                        Name = "Naranja",
                        Description = "Naranja de cosecha tardía",
                        CurrentUnitPrice = 45.00m,
                        StockQuantity = 150,
                    },
                    new Product
                    {
                        ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afb0"),
                        SKU = "FRUT-002",
                        InternalCode = "BAN-001",
                        Name = "Banana",
                        Description = "Banana ecuatoriana",
                        CurrentUnitPrice = 55.00m,
                        StockQuantity = 130,
                    },
                };

                context.Products.AddRange(products);
                context.SaveChanges();
            }

            // 2. Órdenes iniciales (ejemplos realistas)
            if (!context.Orders.Any())
            {
                var products = context.Products.ToList();
                var customerId = Guid.Parse("9fa85f64-5717-4562-b3fc-2c963f66afb0");

                var orders = new List<Order>
                {
                    new Order
                    {
                        OrderId = Guid.Parse("4fa85f64-5717-4562-b3fc-2c963f66afb1"),
                        CustomerId = customerId,
                        OrderDate = DateTime.UtcNow.AddDays(-2),
                        OrderStatus = "Delivered",
                        ShippingAddress = "Av. Siempreviva 742, Springfield",
                        BillingAddress = "Av. Siempreviva 742, Springfield",
                        TotalAmount =
                            products[0].CurrentUnitPrice * 2 + products[1].CurrentUnitPrice * 3,
                        OrderItems = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                OrderItemId = Guid.NewGuid(),
                                ProductId = products[0].ProductId,
                                Quantity = 2,
                                UnitPrice = products[0].CurrentUnitPrice,
                                Subtotal = products[0].CurrentUnitPrice * 2,
                            },
                            new OrderItem
                            {
                                OrderItemId = Guid.NewGuid(),
                                ProductId = products[1].ProductId,
                                Quantity = 3,
                                UnitPrice = products[1].CurrentUnitPrice,
                                Subtotal = products[1].CurrentUnitPrice * 3,
                            },
                        },
                    },
                    new Order
                    {
                        OrderId = Guid.Parse("4fa85f64-5717-4562-b3fc-2c963f66afb2"),
                        CustomerId = customerId,
                        OrderDate = DateTime.UtcNow.AddDays(-1),
                        OrderStatus = "Processing",
                        ShippingAddress = "Calle Falsa 123, Ciudad",
                        BillingAddress = "Calle Falsa 123, Ciudad",
                        TotalAmount =
                            products[3].CurrentUnitPrice * 5 + products[4].CurrentUnitPrice * 2,
                        OrderItems = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                OrderItemId = Guid.NewGuid(),
                                ProductId = products[3].ProductId,
                                Quantity = 5,
                                UnitPrice = products[3].CurrentUnitPrice,
                                Subtotal = products[3].CurrentUnitPrice * 5,
                            },
                            new OrderItem
                            {
                                OrderItemId = Guid.NewGuid(),
                                ProductId = products[4].ProductId,
                                Quantity = 2,
                                UnitPrice = products[4].CurrentUnitPrice,
                                Subtotal = products[4].CurrentUnitPrice * 2,
                            },
                        },
                    },
                    new Order
                    {
                        OrderId = Guid.Parse("4fa85f64-5717-4562-b3fc-2c963f66afb3"),
                        CustomerId = customerId,
                        OrderDate = DateTime.UtcNow,
                        OrderStatus = "Pending",
                        ShippingAddress = "Boulevard Los Incas 456, Lima",
                        BillingAddress = "Boulevard Los Incas 456, Lima",
                        TotalAmount = products[2].CurrentUnitPrice * 10,
                        OrderItems = new List<OrderItem>
                        {
                            new OrderItem
                            {
                                OrderItemId = Guid.NewGuid(),
                                ProductId = products[2].ProductId,
                                Quantity = 10,
                                UnitPrice = products[2].CurrentUnitPrice,
                                Subtotal = products[2].CurrentUnitPrice * 10,
                            },
                        },
                    },
                };

                // Actualizar stock de productos
                foreach (var order in orders)
                {
                    foreach (var item in order.OrderItems)
                    {
                        var product = products.First(p => p.ProductId == item.ProductId);
                        product.StockQuantity -= item.Quantity;
                    }
                }

                context.Orders.AddRange(orders);
                context.SaveChanges();
            }
        }
    }
}
