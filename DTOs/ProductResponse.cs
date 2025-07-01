namespace OrdenesAPI.DTOs;

/// <summary>
/// Representa la información de un producto devuelta por la API
/// </summary>
public class ProductResponse
{
    /// <summary>Identificador único del producto</summary>
    public Guid ProductId { get; set; }

    /// <summary>SKU (Stock Keeping Unit) del producto</summary>
    public string SKU { get; set; } = default!;

    /// <summary>Código interno del producto</summary>
    public string InternalCode { get; set; } = default!;

    /// <summary>Nombre del producto</summary>
    public string Name { get; set; } = default!;

    /// <summary>Descripción del producto</summary>
    public string? Description { get; set; }

    /// <summary>Precio unitario actual</summary>
    public decimal CurrentUnitPrice { get; set; }

    /// <summary>Cantidad en stock</summary>
    public int StockQuantity { get; set; }
}
