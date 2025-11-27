

public class ProductCreateDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Description { get; set; }
    public int? Stock { get; set; }
    public string? UserId { get; set; }
    public IFormFile? Image { get; set; }
    public Guid? CategoryId { get; set; }
}