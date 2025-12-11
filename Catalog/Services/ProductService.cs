using Catalog.Models;
using Catalog.Repositories;

namespace Catalog.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Product>> GetProductsByUserIdAsync(string userId)
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

        /// <summary>
        /// Actualiza el stock de un producto (para reservar stock durante la creación de órdenes)
        /// </summary>
        public async Task<Product> UpdateStockAsync(Guid productId, int stock)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new InvalidOperationException($"Producto con ID {productId} no encontrado");

            if (stock < 0)
                throw new ArgumentException("El stock no puede ser negativo");

            product.Stock = stock;
            var updated = await _productRepository.UpdateAsync(product);

            _logger.LogInformation("Stock actualizado para producto {ProductId}: nuevo stock = {Stock}", productId, stock);

            return updated;
        }

        /// <summary>
        /// Confirma la reserva de stock para una orden (validación final)
        /// </summary>
        public async Task<bool> ConfirmStockReservationAsync(Guid orderId, List<ReservationItem> items)
        {
            try
            {
                if (items == null || !items.Any())
                {
                    _logger.LogWarning("Intento de confirmar reserva sin items para orden {OrderId}", orderId);
                    return false;
                }

                foreach (var item in items)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId);

                    if (product == null)
                    {
                        _logger.LogError("Producto {ProductId} no encontrado al confirmar reserva para orden {OrderId}",
                            item.ProductId, orderId);
                        return false;
                    }

                    // Validar que el stock no sea negativo después de la reserva
                    if (product.Stock < 0)
                    {
                        _logger.LogError("Stock inválido para producto {ProductId}: {Stock}",
                            item.ProductId, product.Stock);
                        return false;
                    }

                    _logger.LogInformation("Reserva confirmada - Orden: {OrderId}, Producto: {ProductId}, Cantidad: {Quantity}",
                        orderId, item.ProductId, item.ReservedQuantity);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al confirmar reserva de stock para orden {OrderId}", orderId);
                return false;
            }
        }
    }

    // DTOs para stock management
    public class ReservationItem
    {
        public Guid ProductId { get; set; }
        public int ReservedQuantity { get; set; }
    }
}