using Catalog.Models;
using Catalog.Repositories;
using Catalog.Services;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Tests
{
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> _repositoryMock = null!;
        private CategoryService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repositoryMock = new Mock<ICategoryRepository>();
            _service = new CategoryService(_repositoryMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllCategories()
        {
            var categories = new List<Category>
            {
                new Category { Id = Guid.NewGuid(), Name = "Books" },
                new Category { Id = Guid.NewGuid(), Name = "Electronics" }
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await _service.GetAllAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
            _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnCategory_WhenExists()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Books" };
            _repositoryMock.Setup(r => r.GetByIdAsync(category.Id)).ReturnsAsync(category);

            var result = await _service.GetByIdAsync(category.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Books"));
            _repositoryMock.Verify(r => r.GetByIdAsync(category.Id), Times.Once);
        }

        [Test]
        public async Task CreateAsync_ShouldReturnCreatedCategory()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Toys" };
            _repositoryMock.Setup(r => r.AddAsync(category)).ReturnsAsync(category);

            var result = await _service.CreateAsync(category);

            Assert.That(result, Is.EqualTo(category));
            _repositoryMock.Verify(r => r.AddAsync(category), Times.Once);
        }

        [Test]
        public async Task UpdateAsync_ShouldReturnUpdatedCategory()
        {
            var category = new Category { Id = Guid.NewGuid(), Name = "Toys" };
            _repositoryMock.Setup(r => r.UpdateAsync(category)).ReturnsAsync(category);

            var result = await _service.UpdateAsync(category);

            Assert.That(result, Is.EqualTo(category));
            _repositoryMock.Verify(r => r.UpdateAsync(category), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnTrue_WhenDeleted()
        {
            var categoryId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(categoryId)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(categoryId);

            Assert.That(result, Is.True);
            _repositoryMock.Verify(r => r.DeleteAsync(categoryId), Times.Once);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
        {
            var categoryId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.DeleteAsync(categoryId)).ReturnsAsync(false);

            var result = await _service.DeleteAsync(categoryId);

            Assert.That(result, Is.False);
            _repositoryMock.Verify(r => r.DeleteAsync(categoryId), Times.Once);
        }
    }
}
