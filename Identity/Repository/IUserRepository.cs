using Identity.Models;

namespace Identity.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(User user, string password);
        Task<User?> UpdateAsync(string id, User user);
        Task<bool> DeleteAsync(string id);
    }

}
