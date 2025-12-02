using MassTransit;
using Microsoft.Extensions.Logging;
using Notifications.Services;
using Shared.Events;

public class UserRegisteredConsumer(
    IEmailService emailService,
    ILogger<UserRegisteredConsumer> logger) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var user = context.Message;
        var @event = context.Message;

        logger.LogInformation(
            "Processing UserRegisteredEvent: EventId={EventId}, UserId={UserId}, Email={Email}",
            @event.EventId, @event.UserId, @event.Email);

        await emailService.SendWelcomeEmailAsync(
            @event.Email,
            @event.Nombre,
            context.CancellationToken);

        logger.LogInformation(
            "Successfully processed UserRegisteredEvent: EventId={EventId}",
            @event.EventId);
    }
}