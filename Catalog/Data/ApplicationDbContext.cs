using Catalog.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ============================
            // CONFIGURACIÓN DE PRODUCT
            // ============================
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");

                entity.Property(p => p.Name)
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(p => p.Price)
                      .HasColumnType("decimal(18,2)");

                entity.Property(p => p.Description)
                      .HasMaxLength(500);

                // UserId queda como un campo simple, no relación
                // Relación opcional con Category
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.SetNull);

                // 1 Product → N Chats
                entity.HasMany(p => p.Chats)
                      .WithOne(c => c.Product)
                      .HasForeignKey(c => c.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================
            // CONFIGURACIÓN DE CATEGORY
            // ============================
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");

                entity.Property(c => c.Name)
                      .HasMaxLength(100)
                      .IsRequired();
            });

            // ============================
            // CONFIGURACIÓN DE CHAT
            // ============================
            modelBuilder.Entity<Chat>(entity =>
            {
                entity.ToTable("Chats");

                entity.Property(c => c.BuyerId).IsRequired();
                entity.Property(c => c.SellerId).IsRequired();
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("NOW()");

                // 1 Chat → N Messages
                entity.HasMany(c => c.Messages)
                      .WithOne(m => m.Chat)
                      .HasForeignKey(m => m.ChatId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ============================
            // CONFIGURACIÓN DE MESSAGE
            // ============================
            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");

                entity.Property(m => m.Content)
                      .HasMaxLength(500)
                      .IsRequired();

                entity.Property(m => m.SenderId).IsRequired();
            });
        }
    }
}
