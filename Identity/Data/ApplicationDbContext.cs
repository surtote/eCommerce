using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración adicional de User
            modelBuilder.Entity<User>(entity =>
            {
                // Campos Identity
                entity.Property(u => u.UserName)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(u => u.Email)
                      .HasMaxLength(100)
                      .IsRequired();

                // Campos personalizados
                entity.Property(u => u.Nombre)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(u => u.Apellido)
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(u => u.Dni)
                      .HasMaxLength(9)
                      .IsRequired();

                entity.Property(u => u.Telefono)
                      .HasMaxLength(9); // Opcional

                entity.Property(u => u.Direccion)
                      .HasMaxLength(200); // Opcional

            });
        }
    }
}
