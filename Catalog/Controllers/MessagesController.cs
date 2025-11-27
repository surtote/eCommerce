using Catalog.DTO;
using Catalog.Models;
using Catalog.Services;

using Microsoft.AspNetCore.Mvc;


namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessageService _service;

        public MessagesController(IMessageService service)
        {
            _service = service;
        }

        [HttpGet("chat/{chatId}")]
        public async Task<IActionResult> GetByChatId(Guid chatId)
        {
            var messages = await _service.GetByChatIdAsync(chatId);

            var result = messages.Select(m => new MessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                SenderId = m.SenderId,
                //SenderName = m.Sender?.Nombre,
                Content = m.Content,
                SentAt = m.SentAt
            });

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageCreateDto dto)
        {
            var message = new Message
            {
                ChatId = dto.ChatId,
                SenderId = dto.SenderId,
                Content = dto.Content
            };

            var created = await _service.CreateAsync(message);

            var result = new MessageDto
            {
                Id = created.Id,
                ChatId = created.ChatId,
                SenderId = created.SenderId,
                //SenderName = created.Sender?.Nombre,
                Content = created.Content,
                SentAt = created.SentAt
            };

            return CreatedAtAction(nameof(GetByChatId), new { chatId = created.ChatId }, result);
        }
    }
}
