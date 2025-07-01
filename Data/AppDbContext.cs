using Microsoft.EntityFrameworkCore;
using OrdenesAPI.Models;

namespace OrdenesAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Precisión decimal para montos
            modelBuilder.Entity<Product>().Property(p => p.CurrentUnitPrice).HasPrecision(18, 2);

            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>().Property(oi => oi.Subtotal).HasPrecision(18, 2);

            // Configuración de relaciones

            modelBuilder
                .Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); // Eliminar ítems si se borra la orden

            modelBuilder
                .Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // No permitir borrar producto si tiene ítems
        }
    }
}
