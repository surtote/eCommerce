using Identity.Services.Common;
using Orders.Models;
using static Orders.DTOs.OrdersDTO;

namespace Orders.Services
{
    public interface IOrderService
    {
        Task<ServiceResult<IEnumerable<OrderListResponse>>> GetUserOrdersAsync(string userId);

        Task<ServiceResult<OrderResponse>> GetByIdAsync(Guid id, string userId);

        Task<ServiceResult<OrderResponse>> CreateAsync(string userId, CreateOrderRequest request);

        Task<ServiceResult> CancelAsync(Guid id, string userId);

        Task<ServiceResult<IEnumerable<OrderListResponse>>> GetAllAsync(
            OrderStatus? status,
            string? userId
        );

        Task<ServiceResult<OrderResponse>> GetByIdForAdminAsync(Guid id);

        Task<ServiceResult> UpdateStatusAsync(Guid id, OrderStatus newStatus);
    }
}
