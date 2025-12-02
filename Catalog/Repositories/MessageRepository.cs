using Catalog.Data;
using Catalog.Models;
using Catalog.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Catalog.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetMessagesByChatIdAsync(Guid chatId)
        {
            return await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message> AddAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // No se puede cargar Sender, solo devolvemos el message con SenderId
            return message;
        }

    }
}
