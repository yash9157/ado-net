using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StoreApi.Models;

namespace StoreApi.Data;

public sealed class StoreDbContext(DbContextOptions<StoreDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products", "dbo");
            entity.HasKey(product => product.ProductId);
            entity.Property(product => product.Name)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(product => product.Price)
                .HasPrecision(10, 2);
            entity.Property(product => product.CreatedUtc)
                .HasDefaultValueSql("SYSUTCDATETIME()");
        });
    }
}
