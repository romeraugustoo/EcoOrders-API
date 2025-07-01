using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OrdenesAPI.DTOs
{
    /// <summary>
    /// DTO para crear una nueva orden.
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// ID del cliente que realiza la orden.
        /// </summary>
        [Required]
        public Guid CustomerId { get; set; }

        /// <summary>
        /// Dirección de envío. Mínimo 5 caracteres.
        /// </summary>
        [Required(ErrorMessage = "La dirección de envío es obligatoria.")]
        [MinLength(5, ErrorMessage = "La dirección de envío debe tener al menos 5 caracteres.")]
        public string ShippingAddress { get; set; } = string.Empty;

        /// <summary>
        /// Dirección de facturación. Mínimo 5 caracteres.
        /// </summary>
        [Required(ErrorMessage = "La dirección de facturación es obligatoria.")]
        [MinLength(
            5,
            ErrorMessage = "La dirección de facturación debe tener al menos 5 caracteres."
        )]
        public string BillingAddress { get; set; } = string.Empty;

        /// <summary>
        /// Notas adicionales para la orden.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Lista de ítems que conforman la orden. Debe contener al menos un ítem.
        /// </summary>
        [Required(ErrorMessage = "Debe incluir al menos un item en la orden.")]
        [MinLength(1, ErrorMessage = "La orden debe tener al menos un ítem.")]
        public List<CreateOrderItemRequest> Items { get; set; } =
            new List<CreateOrderItemRequest>();
    }

    /// <summary>
    /// DTO para un ítem dentro de una orden.
    /// </summary>
    public class CreateOrderItemRequest
    {
        /// <summary>
        /// ID del producto solicitado.
        /// </summary>
        [Required(ErrorMessage = "El ID del producto es obligatorio.")]
        public Guid ProductId { get; set; }

        /// <summary>
        /// Cantidad solicitada del producto. Debe ser mayor que cero.
        /// </summary>
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
        public int Quantity { get; set; }
    }
}
