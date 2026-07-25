using Microsoft.EntityFrameworkCore;
using ProductService.Entities;

namespace ProductService.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Products");
                entity.HasKey(p => p.ProductId);

                entity.Property(p => p.ProductId)
                      .HasColumnName("ProductId")
                      .HasDefaultValueSql("NEWID()");

                entity.Property(p => p.Name)
                      .HasColumnName("ProductName")
                      .HasMaxLength(150)
                      .IsRequired();

                entity.Property(p => p.Price)
                      .HasColumnName("Price")
                      .HasColumnType("decimal(10,2)")
                      .IsRequired();

                entity.Property(p => p.StockQty)
                      .HasColumnName("StockQty")
                      .IsRequired();

                entity.Property(p => p.IsActive)
                      .HasColumnName("IsActive")
                      .HasDefaultValue(true);

                entity.Property(p => p.CreatedAt)
                      .HasColumnName("CreatedAt")
                      .HasDefaultValueSql("SYSUTCDATETIME()");

                entity.Property(p => p.UpdatedAt)
                      .HasColumnName("UpdatedAt");

                entity.Property(p => p.RowVersion)
                      .IsRowVersion();

                entity.HasIndex(p => p.Name).HasDatabaseName("IX_Products_Name");

                entity.ToTable(t => t.HasCheckConstraint("CK_Products_Price", "[Price] >= 0"));
                entity.ToTable(t => t.HasCheckConstraint("CK_Products_StockQty", "[StockQty] >= 0"));
            });
        }
    }
}
