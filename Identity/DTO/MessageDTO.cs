namespace MyApi.DTO
{
    public class MessageDto
    {
        public Guid Id { get; set; }
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string? SenderName { get; set; }   // 🔹 Nombre del remitente
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public class MessageCreateDto
    {
        public Guid ChatId { get; set; }
        public Guid SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
