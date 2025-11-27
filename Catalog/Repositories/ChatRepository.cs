using Catalog.Data;
using Catalog.Models;
using Microsoft.EntityFrameworkCore;


namespace Catalog.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Chat>> GetAllAsync() =>
            await _context.Chats.Include(c => c.Messages).ToListAsync();

        public async Task<Chat?> GetByIdAsync(Guid id) =>
            await _context.Chats.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Chat> AddAsync(Chat chat)
        {
            // Asegurarse de que CreatedAt sea UTC
            chat.CreatedAt = chat.CreatedAt.Kind == DateTimeKind.Utc
                ? chat.CreatedAt
                : chat.CreatedAt.ToUniversalTime();

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            return chat;
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat == null) return false;
            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
