

namespace Catalog.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int? Stock {  get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public byte[]? ImageData { get; set; }
        public string? ImageContentType { get; set; }
        // Relación con usuario
        public string UserId { get; set; }

        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
        public ICollection<Chat> Chats { get; set; } = new List<Chat>();
    }
}
