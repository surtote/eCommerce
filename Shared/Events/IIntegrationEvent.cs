using System;
using System.Collections.Generic;
using System.Text;
using MassTransit;
namespace Shared.Events
{
    [ExcludeFromTopology]
    public interface IIntegrationEvent
    {
        Guid EventId { get; }
        DateTime Timestamp { get; }
    }
}
