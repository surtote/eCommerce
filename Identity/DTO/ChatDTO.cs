namespace MyApi.DTO
{
    public class ChatDto
    {
        public Guid Id { get; set; }

        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid ProductId { get; set; }
        public string? SellerName { get; set; }  // Opcional, para mostrar nombres en las respuestas
        public string? BuyerName { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // DTO para crear un chat
    public class ChatCreateDto
    {
        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }
        public Guid ProductId { get; set; }
    }

    // DTO para actualizar un chat (por si lo necesitas en el futuro)
    public class ChatUpdateDto
    {
        public Guid SellerId { get; set; }
        public Guid BuyerId { get; set; }
    }
}
