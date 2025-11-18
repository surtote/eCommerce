namespace Identity.DTO
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Email { get; set; }
        public int? Telefono { get; set; }
        public string? Direccion { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Campos adicionales de Identity
        public bool EmailConfirmed { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}