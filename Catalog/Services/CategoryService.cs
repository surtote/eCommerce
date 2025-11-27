using Catalog.Models;
using Catalog.Repositories;

namespace Catalog.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<Category>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Category?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);
        public Task<Category> CreateAsync(Category category) => _repository.AddAsync(category);
        public Task<Category> UpdateAsync(Category category) => _repository.UpdateAsync(category);
        public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);
    }
}
