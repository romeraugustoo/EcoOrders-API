using System.ComponentModel.DataAnnotations;

namespace OrdenesAPI.DTOs
{
    /// <summary>
    /// Datos necesarios para crear un nuevo producto
    /// </summary>
    public class CreateProductRequest
    {
        /// <summary>
        /// SKU único del producto (obligatorio)
        /// </summary>
        [Required(ErrorMessage = "El SKU es obligatorio.")]
        public string SKU { get; set; } = string.Empty;

        /// <summary>
        /// Código interno único del producto (obligatorio)
        /// </summary>
        [Required(ErrorMessage = "El código interno es obligatorio.")]
        public string InternalCode { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del producto (obligatorio)
        /// </summary>
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Descripción opcional del producto
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Precio unitario actual del producto, debe ser mayor o igual a 0 (obligatorio)
        /// </summary>
        [Required(ErrorMessage = "El precio unitario es obligatorio.")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio unitario debe ser mayor o igual a 0.")]
        public decimal CurrentUnitPrice { get; set; }

        /// <summary>
        /// Cantidad en stock, debe ser mayor o igual a 0 (opcional, valor por defecto 0)
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad en stock debe ser mayor o igual a 0.")]
        public int StockQuantity { get; set; } = 0;
    }
}
