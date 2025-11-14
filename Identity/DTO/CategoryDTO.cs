namespace MyApi.DTO
{
    // DTO para devolver información de la categoría
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // DTO para crear una nueva categoría
    public class CategoryCreateDto
    {
        public string Name { get; set; } = string.Empty;
    }

    // DTO para actualizar una categoría existente
    public class CategoryUpdateDto
    {
        public string Name { get; set; } = string.Empty;
    }
}
