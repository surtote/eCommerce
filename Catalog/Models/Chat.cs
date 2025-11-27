

namespace Catalog.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid SellerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relaciones
        public Product Product { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }

}
