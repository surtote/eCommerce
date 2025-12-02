using Catalog.DTO;
using Catalog.Models;
using Catalog.Services;
using Microsoft.AspNetCore.Mvc;


namespace Catalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatsController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatsController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // 🔹 GET: api/chats
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var chats = await _chatService.GetAllAsync();

            var chatDtos = chats.Select(c => new ChatDto
            {
                Id = c.Id,
                SellerId = c.SellerId,
                BuyerId = c.BuyerId,
                //SellerName = c.Seller?.Nombre,
                //BuyerName = c.Buyer?.Nombre,
                CreatedAt = c.CreatedAt
            });

            return Ok(chatDtos);
        }

        // 🔹 GET: api/chats/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var chat = await _chatService.GetByIdAsync(id);
            if (chat == null) return NotFound();

            var dto = new ChatDto
            {
                Id = chat.Id,
                SellerId = chat.SellerId,
                BuyerId = chat.BuyerId,
                //SellerName = chat.Seller?.Nombre,
                //BuyerName = chat.Buyer?.Nombre,
                CreatedAt = chat.CreatedAt
            };

            return Ok(dto);
        }

        // 🔹 POST: api/chats
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChatCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newChat = new Chat
            {
                SellerId = dto.SellerId,
                BuyerId = dto.BuyerId,
                ProductId = dto.ProductId, // 👈 incluirlo aquí
                CreatedAt = DateTime.UtcNow
            };

            var created = await _chatService.CreateAsync(newChat);

            var resultDto = new ChatDto
            {
                Id = created.Id,
                SellerId = created.SellerId,
                BuyerId = created.BuyerId,
                ProductId = created.ProductId, // 👈 incluirlo también
                CreatedAt = created.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, resultDto);
        }


        // 🔹 DELETE: api/chats/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _chatService.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
