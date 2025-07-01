using System;
using System.Collections.Generic;

namespace OrdenesAPI.DTOs
{
    public class OrderResponse
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }

        // Marcar como required o inicializar para evitar warning
        public string OrderStatus { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;

        // Opcional, puede ser null
        public string? Notes { get; set; }

        public List<OrderItemResponse> Items { get; set; } = new List<OrderItemResponse>();
    }

    public class OrderItemResponse
    {
        public Guid OrderItemId { get; set; }
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}
