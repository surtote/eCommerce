using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{

    public sealed record UserRegisteredEvent(
        string UserId,
        string Email,
        string? Nombre,
        string? Apellido
    ) : IIntegrationEvent
    {
        public Guid EventId => Guid.NewGuid();
        public DateTime Timestamp => DateTime.UtcNow;
    }
}

