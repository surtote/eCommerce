using Catalog.Models;

namespace Catalog.Repositories
{
    public interface IChatRepository
    {
        Task<IEnumerable<Chat>> GetAllAsync();
        Task<Chat?> GetByIdAsync(Guid id);
        Task<Chat> AddAsync(Chat chat);
        Task<bool> DeleteAsync(Guid id);
    }
}
