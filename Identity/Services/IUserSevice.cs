using Identity.DTO;

namespace Identity.Services
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(string id);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse?> UpdateUserAsync(string id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(string id);
    }
}
