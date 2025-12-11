

using Catalog.Models;

namespace Catalog.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();               
        Task<Product> GetByIdAsync(Guid id);                    
        Task<IEnumerable<Product>> GetByUserIdAsync(string userId); 
        Task<Product> AddAsync(Product product);        
        Task<Product> UpdateAsync(Product product);       
        Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
        Task<bool> DeleteAsync(Guid id);

    }
}
