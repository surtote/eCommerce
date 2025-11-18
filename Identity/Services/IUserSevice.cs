using Identity.DTO;
using Identity.Models.Query;
using Identity.Services.Common;

namespace Identity.Services
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(string id);
        Task<PaginatedResult<UserResponse>> GetUsersAsync(UserQueryParameters parameters);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse?> UpdateUserAsync(string id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(string id);
    }
}
