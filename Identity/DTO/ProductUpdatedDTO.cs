using MyApi.DTO;

namespace MiApi.DTO
{
    public class ProductUpdatedDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public int? Stock { get; set; }
        public Guid? CategoryId { get; set; }
        public CategoryDto? Category { get; set; }
    }
}
