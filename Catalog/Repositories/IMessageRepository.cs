

using Catalog.Models;

namespace Catalog.Repositories
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId);
        Task<Message> AddAsync(Message message);
    }
}
