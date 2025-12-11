using Identity.Services.Common;
using Microsoft.EntityFrameworkCore;
using Orders.Data;
using Orders.Models;
using static Orders.DTOs.OrdersDTO;

namespace Orders.Services;

public class OrderService(
    ApplicationDbContext db,
    IHttpClientFactory httpClientFactory,
    ILogger<OrderService> logger) : Orders.Services.IOrderService
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
        var reservedItems = new List<(Guid ProductId, int Quantity)>();

        // 1. Verificar disponibilidad y reservar stock
        foreach (var item in request.Items)
        {
            try
            {
                // Obtener información del producto
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

                // Verificar si el producto está activo (asumiendo que stock null o 0 significa no disponible)
                if (product.Stock == null || product.Stock <= 0)
                {
                    await ReleaseReservedStockAsync(catalogClient, reservedItems);
                    return ServiceResult<OrderResponse>.Failure($"Product {product.Name} is out of stock");
                }

                if (product.Stock < item.Quantity)
                {
                    await ReleaseReservedStockAsync(catalogClient, reservedItems);
                    return ServiceResult<OrderResponse>.Failure($"Insufficient stock for {product.Name}. Available: {product.Stock}, Requested: {item.Quantity}");
                }

                // Actualizar stock - reservar restando la cantidad
                var updateStockRequest = new
                {
                    Stock = product.Stock - item.Quantity,
                    IsReserved = true
                };

                var updateResponse = await catalogClient.PutAsJsonAsync(
                    $"/api/products/{item.ProductId}/stock",
                    updateStockRequest);

                if (!updateResponse.IsSuccessStatusCode)
                {
                    await ReleaseReservedStockAsync(catalogClient, reservedItems);
                    var error = await updateResponse.Content.ReadAsStringAsync();

                    // Agregar más logging
                    logger.LogError(
                        "Failed to update stock. Status: {StatusCode}, Response: {Response}",
                        updateResponse.StatusCode,
                        error);

                    return ServiceResult<OrderResponse>.Failure(
                        $"Failed to reserve stock for {product.Name}: {error}");
                }
                reservedItems.Add((item.ProductId, item.Quantity));

                orderItems.Add(new OrderProduct
                {
                    ProductId = item.ProductId,
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
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error processing product {ProductId}", item.ProductId);
                await ReleaseReservedStockAsync(catalogClient, reservedItems);
                return ServiceResult<OrderResponse>.Failure($"Error processing product {item.ProductId}");
            }
        }

        // 2. Crear la orden en la base de datos
        var order = new Order
        {
            UserId = userId,
            ShippingAddress = request.ShippingAddress,
            Notes = request.Notes,
            OrderProducts = orderItems,
            TotalAmount = orderItems.Sum(i => i.UnitPrice * i.Quantity),
            Status = OrderStatus.Pending
        };

        try
        {
            db.Orders.Add(order);
            await db.SaveChangesAsync();

            logger.LogInformation("Order created: {OrderId} for user {UserId}", order.Id, userId);

            // 3. Confirmar la reserva en el catálogo
            await ConfirmStockReservationAsync(catalogClient, order, reservedItems);

            // 4. Actualizar el estado de la orden a confirmada
            order.Status = OrderStatus.Confirmed;
            order.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            return ServiceResult<OrderResponse>.Success(MapToResponse(order), "Order created successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create order in database");

            // Si falla la creación de la orden, liberar el stock reservado
            await ReleaseReservedStockAsync(catalogClient, reservedItems);

            return ServiceResult<OrderResponse>.Failure("Failed to create order. Please try again.");
        }
    }

    private async Task ConfirmStockReservationAsync(HttpClient catalogClient, Order order, List<(Guid ProductId, int Quantity)> reservedItems)
    {
        try
        {
            // Aquí podrías marcar el stock como definitivamente vendido en lugar de solo reservado
            var confirmResponse = await catalogClient.PostAsJsonAsync(
                "/api/products/confirm-reservation",
                new
                {
                    OrderId = order.Id,
                    Items = reservedItems.Select(item => new
                    {
                        ProductId = item.ProductId,
                        ReservedQuantity = item.Quantity
                    })
                });

            if (!confirmResponse.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to confirm stock reservation for order {OrderId}", order.Id);
                // Aquí podrías implementar lógica de reintento o compensación
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error confirming stock reservation for order {OrderId}", order.Id);
        }
    }

    private async Task ReleaseReservedStockAsync(HttpClient catalogClient, List<(Guid ProductId, int Quantity)> reservedItems)
    {
        foreach (var (productId, quantity) in reservedItems)
        {
            try
            {
                // Primero obtener el stock actual
                var productResponse = await catalogClient.GetAsync($"/api/products/{productId}");
                if (productResponse.IsSuccessStatusCode)
                {
                    var product = await productResponse.Content.ReadFromJsonAsync<ProductInfo>();
                    if (product != null)
                    {
                        // Liberar stock sumando la cantidad reservada
                        var releaseStockRequest = new
                        {
                            Stock = (product.Stock ?? 0) + quantity
                        };

                        await catalogClient.PutAsJsonAsync(
                            $"/api/products/{productId}/stock",
                            releaseStockRequest);

                        logger.LogInformation("Released {Quantity} units of product {ProductId}", quantity, productId);
                    }
                }
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
        var itemsToRelease = order.OrderProducts
            .Select(op => (op.ProductId, op.Quantity))
            .ToList();

        // Liberar el stock en el catálogo
        await ReleaseReservedStockAsync(catalogClient, itemsToRelease);

        // Actualizar el estado de la orden
        order.Status = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        logger.LogInformation("Order {OrderId} cancelled by user {UserId}", id, userId);

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

    public async Task<ServiceResult> UpdateStatusAsync(Guid id, OrderStatus newStatus)
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

    // Modelo que coincide con tu Product de catálogo
    private class ProductInfo
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageContentType { get; set; }
        public string UserId { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
    }
}