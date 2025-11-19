using Identity.DTO;
using Identity.Models.Query;
using Identity.Services.Common;

namespace Identity.Services
{
    public interface IUserService
    {
        Task<UserResponse?> GetUserByIdAsync(string id);
        Task<PaginatedResult<UserResponse>> GetUsersAsync(UserQueryParameters parameters);
        Task<ServiceResult> LockUserAsync(string userId, DateTimeOffset? lockoutEnd);
        Task<ServiceResult> UnlockUserAsync(string userId);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<ServiceResult<IEnumerable<string>>> GetUserRolesAsync(string userId);
        Task<ServiceResult> AddUserToRoleAsync(string userId, string roleName);

        /// <summary>
        /// Remove role from user
        /// </summary>
        Task<ServiceResult> RemoveUserFromRoleAsync(string userId, string roleName);
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse?> UpdateUserAsync(string id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(string id);
    }
}
