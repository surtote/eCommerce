

using Catalog.Models;

namespace Catalog.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product> GetProductByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetProductsByUserIdAsync(string userId);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId);
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(Guid id);
        Task<Product> UpdateStockAsync(Guid productId, int stock);
        Task<bool> ConfirmStockReservationAsync(Guid orderId, List<ReservationItem> items);
    }
}
