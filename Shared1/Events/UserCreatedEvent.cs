using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public sealed record UserRegisteredEvent(
       string UserId,
       string Email,
       string? FirstName,
       string? LastName) : IIntegrationEvent
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    }
}
