namespace OrdenesAPI.DTOs
{
    /// <summary>
    /// Respuesta paginada genérica para colecciones.
    /// </summary>
    /// <typeparam name="T">Tipo de elementos en la lista.</typeparam>
    public class PaginatedResponse<T>
    {
        /// <summary>
        /// Número de página actual.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Cantidad de elementos por página.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total de elementos disponibles.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Lista de elementos en la página actual.
        /// </summary>
        public List<T> Items { get; set; } = new List<T>();
    }
}
