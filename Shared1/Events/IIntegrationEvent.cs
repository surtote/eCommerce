using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Events
{
    public interface IIntegrationEvent
    {
        Guid EventId { get; }
        DateTime Timestamp { get; }
    }
}
