using System.ComponentModel.DataAnnotations;

namespace Identity.DTO
{
    public class CreateUserRequest
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        public string Dni { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int? Telefono { get; set; }
        public string? Direccion { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}