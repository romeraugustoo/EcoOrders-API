using OrdenesAPI.DTOs;

namespace OrdenesAPI.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<OrderResponse?> GetOrderByIdAsync(Guid orderId);
        Task<PaginatedResponse<OrderResponse>> GetOrdersAsync(
            string? status,
            Guid? customerId,
            int pageNumber,
            int pageSize
        );
        Task<OrderResponse> UpdateOrderStatusAsync(Guid orderId, string newStatus);
    }
}
