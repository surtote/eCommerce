using Orders.Models;

namespace Orders.DTOs
{
    public static class OrdersDTO
    {
        public record CreateOrderRequest(
            string ShippingAddress,
            string? Notes,
            List<OrderItemRequest> Items);

        public record OrderItemRequest(
            Guid ProductId,
            int Quantity);

        public record UpdateOrderStatusRequest(
            OrderStatus Status);

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

        public record OrderItemResponse(
            Guid Id,
            Guid ProductId,
            string ProductName,
            decimal UnitPrice,
            int Quantity,
            decimal Subtotal);

        public record OrderListResponse(
            Guid Id,
            string Status,
            decimal TotalAmount,
            int ItemCount,
            DateTime CreatedAt);
    }
}