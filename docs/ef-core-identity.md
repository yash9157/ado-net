# 2. Add EF Core and Identity

EF Core maps the Product model to a SQL Server table. Identity uses the same `DbContext` to store users and password hashes. Later, ADO.NET—not EF Core—will handle product requests.

## Product model

The product has an ID, name, price, and database generated creation time:

```csharp
namespace StoreApi.Models;

public sealed class Product
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedUtc { get; set; }
}
```

## Database context

`IdentityDbContext<IdentityUser>` adds Identity tables. `DbSet<Product>` adds the product table. `OnModelCreating` sets the SQL table and column rules.

```csharp
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
```

## How the table migration is created

Install the EF Core command line tool if `dotnet ef --version` does not work:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.12
```

In the [final step](web-api-example.md#try-the-api), run `dotnet ef migrations add InitialStore` after all classes are in place. EF Core generates the migration for `Products` and the `AspNet*` Identity tables. You do not have to type the generated table code.

**Next:** [Add register, login and JWT](authentication.md).
