namespace Catalog.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;

        // Relaciones
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }

}
