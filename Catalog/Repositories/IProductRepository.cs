

using Catalog.Models;

namespace Catalog.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();               // Todos los productos
        Task<Product> GetByIdAsync(Guid id);                     // Producto por Id
        Task<IEnumerable<Product>> GetByUserIdAsync(Guid userId); // Productos de un usuario
        Task<Product> AddAsync(Product product);               // Crear producto
        Task<Product> UpdateAsync(Product product);            // Actualizar producto
        Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
        Task<bool> DeleteAsync(Guid id);

    }
}
