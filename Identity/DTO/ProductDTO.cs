using MyApi.DTO;

namespace MiApi.DTO
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
        public Guid UserId { get; set; }
        public string? ImageData { get; set; }
        public string? ImageContentType { get; set; }

        public Guid? CategoryId { get; set; }
        public CategoryDto? Category { get; set; }

        public string? CategoryName { get; set; }  

        public DateTime CreatedAt { get; set; }
    }
}
