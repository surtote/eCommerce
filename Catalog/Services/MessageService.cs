

using Catalog.Models;
using Catalog.Repositories;
using Catalog.Services;

namespace MyApi.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;

        public MessageService(IMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(Guid chatId)
        {
            // Aquí podrías añadir validaciones o lógica adicional si es necesario
            return await _repository.GetMessagesByChatIdAsync(chatId);
        }

        public async Task<Message> CreateAsync(Message message)
        {
            // Ejemplo: validar contenido vacío o longitud máxima
            if (string.IsNullOrWhiteSpace(message.Content))
                throw new ArgumentException("El mensaje no puede estar vacío.");

            return await _repository.AddAsync(message);
        }
    }
}
