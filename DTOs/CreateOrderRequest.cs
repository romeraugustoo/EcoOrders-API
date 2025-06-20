using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrdenesAPI.DTOs
{
    public class CreateOrderRequest
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public string ShippingAddress { get; set; }

        [Required]
        public string BillingAddress { get; set; }

        [Required]
        public List<OrderItemRequest> OrderItems { get; set; } = new List<OrderItemRequest>();
    }

    public class OrderItemRequest
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
