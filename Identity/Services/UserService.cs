using Identity.DTO;
using Identity.Models;
using Identity.Models.Query;
using Identity.Repository;
using Identity.Services.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;


        public UserService(IUserRepository userRepository, UserManager<User> userManager, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<UserResponse?> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return MapToResponse(user);
        }
        public async Task<PaginatedResult<UserResponse>> GetUsersAsync(UserQueryParameters parameters)
        {
            // Start with all users query
            var query = _userManager.Users.AsQueryable();

            // Apply search filter (email or username)
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                var searchLower = parameters.Search.ToLower();
                query = query.Where(u =>
                    (u.Email != null && u.Email.ToLower().Contains(searchLower)) ||
                    (u.UserName != null && u.UserName.ToLower().Contains(searchLower)));
            }

            // Apply role filter if specified
            if (!string.IsNullOrWhiteSpace(parameters.Role))
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(parameters.Role);
                var userIds = usersInRole.Select(u => u.Id).ToList();
                query = query.Where(u => userIds.Contains(u.Id));
            }

            // Apply sorting (default to email ascending if no sort specified)
            var sortBy = parameters.SortBy?.ToLower() ?? "email";
            var sortDescending = parameters.SortDescending ?? false;
            query = sortBy switch
            {
                "username" => sortDescending
                    ? query.OrderByDescending(u => u.UserName)
                    : query.OrderBy(u => u.UserName),
                _ => sortDescending
                    ? query.OrderByDescending(u => u.Email)
                    : query.OrderBy(u => u.Email)
            };

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var pageSize = Math.Min(parameters.PageSize, 100); // Max 100 items
            var page = Math.Max(parameters.Page, 1); // Min page 1
            var skip = (page - 1) * pageSize;

            var users = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            // Map to response DTOs
            var userResponses = new List<UserResponse>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userResponses.Add(new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnd = user.LockoutEnd,
                    LockoutEnabled = user.LockoutEnabled,
                    AccessFailedCount = user.AccessFailedCount,
                    Roles = roles
                });
            }

            return PaginatedResult<UserResponse>.Create(userResponses, page, pageSize, totalCount);
        }
        public async Task<ServiceResult> LockUserAsync(string userId, DateTimeOffset? lockoutEnd)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult.Failure("User not found");
            }

            // If no lockout end specified, lock for 100 years (effectively permanent)
            var lockEnd = lockoutEnd ?? DateTimeOffset.UtcNow.AddYears(100);

            var lockResult = await _userManager.SetLockoutEndDateAsync(user, lockEnd);
            if (!lockResult.Succeeded)
            {
                var errors = lockResult.Errors.Select(e => e.Description);
                return ServiceResult.Failure(errors);
            }

            _logger.LogInformation("User locked successfully: {UserId} until {LockoutEnd}",
                userId, lockEnd);

            return ServiceResult.Success("User account locked successfully");
        }

        /// <summary>
        /// Unlock user account
        /// </summary>
        public async Task<ServiceResult> UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult.Failure("User not found");
            }

            // Clear lockout
            var unlockResult = await _userManager.SetLockoutEndDateAsync(user, null);
            if (!unlockResult.Succeeded)
            {
                var errors = unlockResult.Errors.Select(e => e.Description);
                return ServiceResult.Failure(errors);
            }

            // Reset access failed count
            var resetResult = await _userManager.ResetAccessFailedCountAsync(user);
            if (!resetResult.Succeeded)
            {
                _logger.LogWarning("Failed to reset access failed count for user {UserId}", userId);
            }

            _logger.LogInformation("User unlocked successfully: {UserId}", userId);

            return ServiceResult.Success("User account unlocked successfully");
        }

        /// <summary>
        /// Get current user's full profile
        /// </summary>
        public async Task<ServiceResult<IEnumerable<string>>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult<IEnumerable<string>>.Failure("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return ServiceResult<IEnumerable<string>>.Success(roles);
        }
        public async Task<ServiceResult> AddUserToRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult.Failure("User not found");
            }

            // Check if role exists
            var roleExists = await _userManager.GetRolesAsync(user);
            if (roleExists.Contains(roleName))
            {
                return ServiceResult.Failure("User already has this role");
            }

            var addResult = await _userManager.AddToRoleAsync(user, roleName);
            if (!addResult.Succeeded)
            {
                var errors = addResult.Errors.Select(e => e.Description);
                return ServiceResult.Failure(errors);
            }

            _logger.LogInformation("Role {Role} assigned to user {UserId}", roleName, userId);

            return ServiceResult.Success($"Role '{roleName}' assigned successfully");
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        public async Task<ServiceResult> RemoveUserFromRoleAsync(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                return ServiceResult.Failure("User not found");
            }

            // Check if user has the role
            var hasRole = await _userManager.IsInRoleAsync(user, roleName);
            if (!hasRole)
            {
                return ServiceResult.Failure("User does not have this role");
            }

            // Prevent removing last role
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Count <= 1)
            {
                return ServiceResult.Failure("Cannot remove the last role from user. Users must have at least one role.");
            }

            var removeResult = await _userManager.RemoveFromRoleAsync(user, roleName);
            if (!removeResult.Succeeded)
            {
                var errors = removeResult.Errors.Select(e => e.Description);
                return ServiceResult.Failure(errors);
            }

            _logger.LogInformation("Role {Role} removed from user {UserId}", roleName, userId);

            return ServiceResult.Success($"Role '{roleName}' removed successfully");
        }
        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToResponse);
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var user = new User
            {
                UserName = request.UserName,
                Nombre = request.Nombre,
                Apellido = request.Apellido,
                Dni = request.Dni,
                Email = request.Email,
                Direccion = request.Direccion,
                Telefono = request.Telefono
            };

            var createdUser = await _userRepository.CreateAsync(user, request.Password);
            return MapToResponse(createdUser);
        }

        public async Task<UserResponse?> UpdateUserAsync(string id, UpdateUserRequest request)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null) return null;

            existingUser.UserName = request.UserName;
            existingUser.Nombre = request.Nombre;
            existingUser.Apellido = request.Apellido;
            existingUser.Dni = request.Dni;
            existingUser.Email = request.Email;
            existingUser.Telefono = request.Telefono;
            existingUser.Direccion = request.Direccion;

            if (!string.IsNullOrEmpty(request.Password))
                existingUser.PasswordHash = _passwordHasher.HashPassword(existingUser, request.Password);

            var updatedUser = await _userRepository.UpdateAsync(id, existingUser);
            if (updatedUser == null) return null;

            return MapToResponse(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        private UserResponse MapToResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Dni = user.Dni,
                Email = user.Email!,
                Telefono = user.Telefono,
                Direccion = user.Direccion
            };
        }
    }
}
