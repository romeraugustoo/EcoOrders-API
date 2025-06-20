using System.ComponentModel.DataAnnotations;

namespace OrdenesAPI.DTOs
{
    public class CreateProductRequest
    {
        [Required]
        public string SKU { get; set; }

        [Required]
        public string InternalCode { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal CurrentUnitPrice { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;
    }
}
