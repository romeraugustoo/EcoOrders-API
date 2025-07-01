using System.ComponentModel.DataAnnotations;

namespace OrdenesAPI.DTOs
{
    /// <summary>
    /// DTO para actualizar campos de un producto existente.
    /// </summary>
    public class UpdateProductRequest
    {
        /// <summary>
        /// Descripción del producto. Campo opcional.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Precio unitario actual del producto. Debe ser mayor o igual a 0. Campo opcional.
        /// </summary>
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        public decimal? CurrentUnitPrice { get; set; }

        /// <summary>
        /// Cantidad en stock del producto. No puede ser negativa. Campo opcional.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad en stock no puede ser negativa")]
        public int? StockQuantity { get; set; }
    }
}
