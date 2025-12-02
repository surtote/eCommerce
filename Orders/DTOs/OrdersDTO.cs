using Orders.Models;

namespace Orders.DTOs
{
    public class OrdersDTO
    {
        public record OrderResponse(
            Guid Id,
            string UserId,
            string Status,
            decimal TotalAmount,
            string? ShippingAddress,
            string? Notes,
            DateTime CreatedAt,
            DateTime? UpdatedAt,
            IEnumerable<OrderItemResponse> Items);

        public record OrderListResponse(
            Guid Id,
            string Status,
            decimal TotalAmount,
            int ItemCount,
            DateTime CreatedAt);

        public record OrderItemResponse(
            Guid Id,
            Guid ProductId,
            string ProductName,
            decimal UnitPrice,
            int Quantity,
            decimal Subtotal);

        public record CreateOrderRequest(
            string? ShippingAddress,
            string? Notes,
            IEnumerable<CreateOrderItemRequest> Items);

        public record CreateOrderItemRequest(
            Guid ProductId,
            int Quantity);

        public record UpdateOrderStatusRequest(OrderStatus Status);
    }
}
