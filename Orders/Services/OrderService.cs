using Identity.Services.Common;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Models;
using Shared.Events;
using static Orders.DTOs.OrdersDTO;

namespace OrderFlow.Orders.Services;

public class OrderService(
    ApplicationDbContext db,
    IHttpClientFactory httpClientFactory,
    IPublishEndpoint publishEndpoint,
    ILogger<OrderService> logger) : IOrderService
{
    public async Task<ServiceResult<IEnumerable<OrderListResponse>>> GetUserOrdersAsync(string userId)
    {
        var orders = await db.Orders
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderListResponse(
                o.Id, o.Status.ToString(), o.TotalAmount, o.OrderProducts.Count, o.CreatedAt))
            .ToListAsync();

        return ServiceResult<IEnumerable<OrderListResponse>>.Success(orders);
    }

    public async Task<ServiceResult<OrderResponse>> GetByIdAsync(Guid id, string userId)
    {
        var order = await db.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            return ServiceResult<OrderResponse>.Failure("Order not found");

        if (order.UserId != userId)
            return ServiceResult<OrderResponse>.Failure("Access denied");

        return ServiceResult<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<ServiceResult<OrderResponse>> CreateAsync(string userId, CreateOrderRequest request)
    {
        if (!request.Items.Any())
            return ServiceResult<OrderResponse>.Failure("Order must have at least one item");

        var catalogClient = httpClientFactory.CreateClient("catalog");
        var orderItems = new List<OrderProduct>();
        var reservedItems = new List<(Guid ProductId, int Quantity)>(); // <-- Guid

        foreach (var item in request.Items)
        {
            try
            {
                var response = await catalogClient.GetAsync($"/api/products/{item.ProductId}");
                if (!response.IsSuccessStatusCode)
                {
                    await ReleaseReservedStockAsync(catalogClient, reservedItems);
                    return ServiceResult<OrderResponse>.Failure($"Product {item.ProductId} not found");
                }

                var product = await response.Content.ReadFromJsonAsync<ProductInfo>();
                if (product is null)
                {
                    await ReleaseReservedStockAsync(catalogClient, reservedItems);
                    return ServiceResult<OrderResponse>.Failure($"Could not fetch product {item.ProductId}");
                }

                if (!product.IsActive)
                {
                    await ReleaseReservedStockAsync(catalogClient, reservedItems);
                    return ServiceResult<OrderResponse>.Failure($"Product {product.Name} is not available");
                }

                var reserveResponse = await catalogClient.PostAsJsonAsync(
                    $"/api/products/{item.ProductId}/reserve",
                    new { Quantity = item.Quantity });

                if (!reserveResponse.IsSuccessStatusCode)
                {
                    await ReleaseReservedStockAsync(catalogClient, reservedItems);
                    var error = await reserveResponse.Content.ReadAsStringAsync();
                    return ServiceResult<OrderResponse>.Failure(
                        reserveResponse.StatusCode == System.Net.HttpStatusCode.Conflict
                            ? $"Insufficient stock for {product.Name}"
                            : $"Failed to reserve stock for {product.Name}: {error}");
                }

                reservedItems.Add((item.ProductId, item.Quantity));

                orderItems.Add(new OrderProduct
                {
                    ProductId = item.ProductId, // <-- Guid
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = item.Quantity
                });
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Catalog service unavailable");
                await ReleaseReservedStockAsync(catalogClient, reservedItems);
                return ServiceResult<OrderResponse>.Failure("Catalog service unavailable");
            }
        }

        var order = new Order
        {
            UserId = userId,
            ShippingAddress = request.ShippingAddress,
            Notes = request.Notes,
            OrderProducts = orderItems,
            TotalAmount = orderItems.Sum(i => i.UnitPrice * i.Quantity),
            Status = OrderStatus.Pending
        };

        db.Orders.Add(order);
        await db.SaveChangesAsync();

        logger.LogInformation("Order created: {OrderId} for user {UserId}", order.Id, userId);

        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            userId,
            orderItems.Select(i => new OrderItemEvent(i.ProductId, i.ProductName, i.Quantity)));

        await publishEndpoint.Publish(orderCreatedEvent);

        return ServiceResult<OrderResponse>.Success(MapToResponse(order), "Order created successfully");
    }

    private async Task ReleaseReservedStockAsync(HttpClient catalogClient, List<(Guid ProductId, int Quantity)> reservedItems)
    {
        foreach (var (productId, quantity) in reservedItems)
        {
            try
            {
                await catalogClient.PostAsJsonAsync(
                    $"/api/v1/products/{productId}/release",
                    new { Quantity = quantity });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to release reserved stock for product {ProductId}", productId);
            }
        }
    }

    public async Task<ServiceResult> CancelAsync(Guid id, string userId)
    {
        var order = await db.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
            return ServiceResult.Failure("Order not found");

        if (order.UserId != userId)
            return ServiceResult.Failure("Access denied");

        if (order.Status is not (OrderStatus.Pending or OrderStatus.Confirmed))
            return ServiceResult.Failure("Order cannot be cancelled at this stage");

        var catalogClient = httpClientFactory.CreateClient("catalog");
        foreach (var item in order.OrderProducts)
        {
            try
            {
                await catalogClient.PostAsJsonAsync(
                    $"/api/v1/products/{item.ProductId}/release",
                    new { Quantity = item.Quantity });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error releasing stock for product {ProductId} on order {OrderId}", item.ProductId, id);
            }
        }

        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        var orderCancelledEvent = new OrderCancelledEvent(
            order.Id,
            userId,
            order.OrderProducts.Select(i => new OrderItemEvent(i.ProductId, i.ProductName, i.Quantity)));

        await publishEndpoint.Publish(orderCancelledEvent);

        return ServiceResult.Success("Order cancelled successfully");
    }
    public async Task<ServiceResult<IEnumerable<OrderListResponse>>> GetAllAsync(OrderStatus? status, string? userId)
    {
        var query = db.Orders.AsQueryable();

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(o => o.UserId == userId);

        var orders = await query
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderListResponse(
                o.Id, o.Status.ToString(), o.TotalAmount, o.OrderProducts.Count, o.CreatedAt))
            .ToListAsync();

        return ServiceResult<IEnumerable<OrderListResponse>>.Success(orders);
    }

    public async Task<ServiceResult<OrderResponse>> GetByIdForAdminAsync(Guid id)
    {
        var order = await db.Orders
            .Include(o => o.OrderProducts)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null)
        {
            return ServiceResult<OrderResponse>.Failure("Order not found");
        }

        return ServiceResult<OrderResponse>.Success(MapToResponse(order));
    }

    public async Task<ServiceResult> UpdateStatusAsync(int id, OrderStatus newStatus)
    {
        var order = await db.Orders.FindAsync(id);
        if (order is null)
        {
            return ServiceResult.Failure("Order not found");
        }

        if (!IsValidStatusTransition(order.Status, newStatus))
        {
            return ServiceResult.Failure($"Cannot transition from {order.Status} to {newStatus}");
        }

        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        logger.LogInformation("Order status updated: {OrderId} -> {Status}", id, newStatus);

        return ServiceResult.Success("Order status updated successfully");
    }

    private static bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
    {
        return (current, next) switch
        {
            (OrderStatus.Pending, OrderStatus.Confirmed) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Processing) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            (OrderStatus.Processing, OrderStatus.Shipped) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false
        };
    }

    private static OrderResponse MapToResponse(Order order) => new(
        order.Id,
        order.UserId,
        order.Status.ToString(),
        order.TotalAmount,
        order.ShippingAddress,
        order.Notes,
        order.CreatedAt,
        order.UpdatedAt,
        order.OrderProducts.Select(i => new OrderItemResponse(
            i.Id, i.ProductId, i.ProductName, i.UnitPrice, i.Quantity, i.Subtotal)));

    private record ProductInfo(int Id, string Name, decimal Price, int Stock, bool IsActive);
}