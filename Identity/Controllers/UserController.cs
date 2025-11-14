using Identity.DTO;
using Identity.Models;
using Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Identity.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public UserController(IUserService userService, IConfiguration configuration, UserManager<User> userManager)
        {
            _userService = userService;
            _configuration = configuration;
            _userManager = userManager;
        }

        // ✅ GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // ✅ GET: api/user/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(user);
        }

        // ✅ POST: api/user
        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ PUT: api/user/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> Update(string id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, request);
                if (updatedUser == null)
                    return NotFound(new { message = "Usuario no encontrado" });

                return Ok(updatedUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ DELETE: api/user/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound(new { message = "Usuario no encontrado" });

            return NoContent();
        }

        // ✅ POST: api/user/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName)
                       ?? await _userManager.FindByEmailAsync(loginRequest.UserName);

            if (user == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            // Verificar contraseña
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!passwordValid)
                return Unauthorized(new { message = "Credenciales inválidas" });

            // Mapear minimo a UserResponse
            var userResponse = new UserResponse
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Nombre = "",
                Apellido = "",
                Dni = "",
                Telefono = 0,
                Direccion = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            var token = GenerateJwtToken(userResponse);

            return Ok(new LoginResponse
            {
                Token = token,
                User = userResponse
            });
        }

        // 🔐 Generador de JWT
        private string GenerateJwtToken(UserResponse user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id), // ahora es string
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("nombre", user.Nombre),
                new Claim("apellido", user.Apellido),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs auxiliares
    public class LoginRequest
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserResponse User { get; set; } = default!;
    }
}
