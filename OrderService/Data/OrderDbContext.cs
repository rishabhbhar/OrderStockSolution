using Microsoft.EntityFrameworkCore;
using OrderService.Entities;

namespace OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.HasKey(o => o.OrderId);

                entity.Property(o => o.OrderId)
                      .HasColumnName("OrderId")
                      .HasDefaultValueSql("NEWID()");

                entity.Property(o => o.ProductId)
                      .HasColumnName("ProductId")
                      .IsRequired();

                entity.Property(o => o.Quantity)
                      .HasColumnName("Quantity")
                      .IsRequired();

                entity.Property(o => o.OrderStatus)
                      .HasColumnName("OrderStatus")
                      .HasConversion<string>()
                      .HasMaxLength(30)
                      .HasDefaultValue(OrderStatus.CREATED)
                      .IsRequired();

                entity.Property(o => o.CreatedAt)
                      .HasColumnName("CreatedAt")
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.Property(o => o.ProductName)
                      .HasColumnName("ProductName")
                      .HasMaxLength(150);

                entity.Property(o => o.UnitPrice)
                      .HasColumnName("UnitPrice")
                      .HasColumnType("decimal(10,2)");

                entity.HasIndex(o => o.ProductId).HasDatabaseName("IX_Orders_ProductId");

                entity.ToTable(t => t.HasCheckConstraint("CK_Orders_Quantity", "[Quantity] > 0"));
            });
        }
    }
}
