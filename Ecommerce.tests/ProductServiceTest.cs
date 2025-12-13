using Catalog.Models;
using Catalog.Repositories;
using Catalog.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Tests
{
    public class ProductServiceTests
    {
        private Mock<IProductRepository> _repositoryMock = null!;
        private ProductService _service = null!;
        private Mock<ILogger<ProductService>> _loggerMock = null!;
        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<IProductRepository>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Test]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Product1" },
                new Product { Id = Guid.NewGuid(), Name = "Product2" }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            var result = (await _service.GetAllProductsAsync()).ToList();

            Assert.That(result.Count, Is.EqualTo(2));
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetProductByIdAsync_ShouldReturnProduct_WhenExists()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1" };
            _repositoryMock.Setup(r => r.GetByIdAsync(product.Id)).ReturnsAsync(product);

            var result = await _service.GetProductByIdAsync(product.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(product.Id));
            _repositoryMock.Verify(r => r.GetByIdAsync(product.Id), Times.Once);
        }

        [Test]
        public async Task GetProductsByUserIdAsync_ShouldReturnProductsForUser()
        {
            var userId = "diughqwuiheui1278"; // userId es string
            var products = new List<Product>
    {
        new Product { Id = Guid.NewGuid(), UserId = userId },
        new Product { Id = Guid.NewGuid(), UserId = userId }
    };

            _repositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(products);

            var result = (await _service.GetProductsByUserIdAsync(userId)).ToList();

            Assert.That(result.All(p => p.UserId == userId));
            _repositoryMock.Verify(r => r.GetByUserIdAsync(userId), Times.Once);
        }


        [Test]
        public async Task GetProductsByCategoryAsync_ShouldReturnProductsForCategory()
        {
            var categoryId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), CategoryId = categoryId },
                new Product { Id = Guid.NewGuid(), CategoryId = categoryId }
            };

            _repositoryMock.Setup(r => r.GetByCategoryIdAsync(categoryId)).ReturnsAsync(products);

            var result = (await _service.GetProductsByCategoryAsync(categoryId)).ToList();

            Assert.That(result.All(p => p.CategoryId == categoryId));
            _repositoryMock.Verify(r => r.GetByCategoryIdAsync(categoryId), Times.Once);
        }

        [Test]
        public async Task CreateProductAsync_ShouldCreateProduct_WhenValid()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 };
            _repositoryMock.Setup(r => r.AddAsync(product)).ReturnsAsync(product);

            var result = await _service.CreateProductAsync(product);

            Assert.That(result, Is.EqualTo(product));
            _repositoryMock.Verify(r => r.AddAsync(product), Times.Once);
        }

        [Test]
        public void CreateProductAsync_ShouldThrowException_WhenPriceNegative()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = -5 };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProductAsync(product));
            Assert.That(ex!.Message, Is.EqualTo("El precio no puede ser negativo."));

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Test]
        public void CreateProductAsync_ShouldThrowException_WhenImageTooLarge()
        {
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Product1",
                Price = 100,
                ImageData = new byte[6_000_000] // >5MB
            };

            var ex = Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProductAsync(product));
            Assert.That(ex!.Message, Is.EqualTo("La imagen no puede superar 5MB."));

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        }

        [Test]
        public async Task UpdateProductAsync_ShouldUpdateProduct()
        {
            var product = new Product { Id = Guid.NewGuid(), Name = "Product1", Price = 100 };
            _repositoryMock.Setup(r => r.UpdateAsync(product)).ReturnsAsync(product);

            var result = await _service.UpdateProductAsync(product);

            Assert.That(result, Is.EqualTo(product));
            _repositoryMock.Verify(r => r.UpdateAsync(product), Times.Once);
        }

        [Test]
        public async Task DeleteProductAsync_ShouldReturnTrue_WhenDeleted()
        {
            var productId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(true);

            var result = await _service.DeleteProductAsync(productId);

            Assert.That(result, Is.True);
            _repositoryMock.Verify(r => r.DeleteAsync(productId), Times.Once);
        }

        [Test]
        public async Task DeleteProductAsync_ShouldReturnFalse_WhenNotFound()
        {
            var productId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(productId)).ReturnsAsync(false);

            var result = await _service.DeleteProductAsync(productId);

            Assert.That(result, Is.False);
            _repositoryMock.Verify(r => r.DeleteAsync(productId), Times.Once);
        }
    }
}
