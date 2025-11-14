using Identity.Models;
using Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserRepository(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<User> CreateAsync(User user, string password)
        {
            // Identity generará su propio ID si no lo asignas
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error creando usuario: {errors}");
            }

            return user;
        }

        public async Task<User?> UpdateAsync(string id, User updatedUser)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;
            user.Nombre = updatedUser.Nombre;
            user.Apellido = updatedUser.Apellido;
            user.Dni = updatedUser.Dni;
            user.Telefono = updatedUser.Telefono;
            user.Direccion = updatedUser.Direccion;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error actualizando usuario: {errors}");
            }

            return user;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
