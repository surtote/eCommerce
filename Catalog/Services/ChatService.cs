using Catalog.Models;
using Catalog.Repositories;

namespace Catalog.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _repository;

        public ChatService(IChatRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Chat>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Chat?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);
        public Task<Chat> CreateAsync(Chat chat) => _repository.AddAsync(chat);
        public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);


    }
}
