using Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Identity.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // 🔹 Cadena de conexión "temporal" para diseño
            optionsBuilder.UseNpgsql("Host=localhost;Database=eCommerce;Username=postgres;Password=postgres");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
