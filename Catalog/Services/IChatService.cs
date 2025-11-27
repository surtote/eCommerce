

using Catalog.Models;

namespace Catalog.Services
{
    public interface IChatService
    {
        Task<IEnumerable<Chat>> GetAllAsync();
        Task<Chat?> GetByIdAsync(Guid id);
        Task<Chat> CreateAsync(Chat chat);
        Task<bool> DeleteAsync(Guid id);
    }
}
