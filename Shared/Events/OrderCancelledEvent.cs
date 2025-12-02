using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public sealed record OrderCancelledEvent(
    Guid OrderId,
    string UserId,
    IEnumerable<OrderItemEvent> Items) : IIntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
