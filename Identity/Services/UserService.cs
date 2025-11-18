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
