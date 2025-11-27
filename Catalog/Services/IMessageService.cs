

using Catalog.Models;

namespace Catalog.Services
{
    public interface IMessageService
    {
        Task<IEnumerable<Message>> GetByChatIdAsync(Guid chatId);
        Task<Message> CreateAsync(Message message);
    }
}
