using System;
using System.Collections.Generic;
using System.Text;

namespace Notifications.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string? firstName, CancellationToken cancellationToken = default);
    }
}
