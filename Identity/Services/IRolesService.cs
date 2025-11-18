using Identity.DTO;
using Identity.Models.Common;
using Identity.Models.Roles.Responses;
using Identity.Services.Common;
namespace Identity.Services
{
    public interface IRolesService
    {
        /// <summary>
        /// Get all roles
        /// </summary>
        Task<ServiceResult<IEnumerable<RoleResponse>>> GetAllRolesAsync();

        /// <summary>
        /// Get role details by ID
        /// </summary>
        Task<ServiceResult<RoleDetailResponse>> GetRoleByIdAsync(string roleId);

        /// <summary>
        /// Create a new role
        /// </summary>
        Task<ServiceResult<RoleResponse>> CreateRoleAsync(string roleName);

        /// <summary>
        /// Update role name
        /// </summary>
        Task<ServiceResult<RoleResponse>> UpdateRoleAsync(string roleId, string newRoleName);

        /// <summary>
        /// Delete role
        /// </summary>
        Task<ServiceResult> DeleteRoleAsync(string roleId);

        /// <summary>
        /// Get users in a specific role (paginated)
        /// </summary>
        Task<PaginatedResult<UserResponse>> GetUsersInRoleAsync(string roleId, PaginationQuery pagination);
    }
}
