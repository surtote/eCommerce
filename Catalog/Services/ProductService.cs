using Catalog.Models;
using Catalog.Repositories;

namespace Catalog.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByUserIdAsync(Guid userId)
        {
            return await _productRepository.GetByUserIdAsync(userId);
        }
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(Guid categoryId)
        {
            
            return await _productRepository.GetByCategoryIdAsync(categoryId);
        }
        public async Task<Product> CreateProductAsync(Product product)
        {
            if (product.Price < 0)
                throw new ArgumentException("El precio no puede ser negativo.");

            // Validación de la imagen (opcional)
            if (product.ImageData != null && product.ImageData.Length > 5_000_000)
                throw new ArgumentException("La imagen no puede superar 5MB.");

            return await _productRepository.AddAsync(product);
        }


        public async Task<Product> UpdateProductAsync(Product product)
        {
            return await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            return await _productRepository.DeleteAsync(id);
        }

    }
}
