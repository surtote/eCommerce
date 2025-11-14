using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [MaxLength(9)]
        [Required]
        public string Dni { get; set; }

        [MaxLength(9)]
        public int? Telefono { get; set; }

        public string? Direccion { get; set; }


        // Constructor vacío requerido por EF
        public User() { }

        // Constructor personalizado (opcional)
        public User(string nombre, string apellido, string dni, string email, string direccion, int? telefono)
        {
            Nombre = nombre;
            Apellido = apellido;
            Dni = dni;
            Email = email;
            Direccion = direccion;
            Telefono = telefono;
            UserName = email; // puedes ajustar esto si deseas que el username sea distinto
        }
    }
}
