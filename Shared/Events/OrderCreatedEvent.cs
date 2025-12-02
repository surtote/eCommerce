using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public sealed record OrderCreatedEvent(
        Guid OrderId,
        string UserId,
        IEnumerable<OrderItemEvent> Items) : IIntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }

    public sealed record OrderItemEvent(
        Guid ProductId,
        string ProductName,
        int Quantity);
}
