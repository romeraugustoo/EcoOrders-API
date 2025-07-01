using System.ComponentModel.DataAnnotations;

namespace OrdenesAPI.DTOs
{
    /// <summary>
    /// DTO para actualizar el estado de una orden.
    /// </summary>
    public class UpdateOrderStatusRequest
    {
        /// <summary>
        /// Nuevo estado de la orden. Campo obligatorio.
        /// </summary>
        [Required]
        public string NewStatus { get; set; } = string.Empty;
    }
}
