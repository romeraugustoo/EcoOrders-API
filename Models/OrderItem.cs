using System;
using System.ComponentModel.DataAnnotations;

namespace OrdenesAPI.Models
{
    public class OrderItem
    {
        public Guid OrderItemId { get; set; }

        public Guid OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public Guid ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal Subtotal { get; set; }
    }
}
