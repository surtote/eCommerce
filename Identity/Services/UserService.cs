using Identity.DTO;
using Identity.Models;
using Identity.Repository;
using Microsoft.AspNetCore.Identity;

namespace Identity.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<UserResponse?> GetUserByIdAsync(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            return MapToResponse(user);
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
