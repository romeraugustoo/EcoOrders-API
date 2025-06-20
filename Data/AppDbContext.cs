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
            modelBuilder.Entity<Product>().Property(p => p.CurrentUnitPrice).HasPrecision(18, 2);

            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>().Property(oi => oi.UnitPrice).HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>().Property(oi => oi.Subtotal).HasPrecision(18, 2);
        }
    }
}
