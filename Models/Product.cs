using System;
using System.ComponentModel.DataAnnotations;

namespace OrdenesAPI.Models
{
    public class Product
    {
        public Guid ProductId { get; set; }

        [Required]
        public string SKU { get; set; } = string.Empty;

        [Required]
        public string InternalCode { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public decimal CurrentUnitPrice { get; set; }

        public int StockQuantity { get; set; }
    }
}
