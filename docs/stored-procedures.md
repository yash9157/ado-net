# 4. Create stored procedures automatically

The search procedure filters products by minimum price. The create procedure inserts a product and returns its new ID. EF Core migrations create both procedures; ADO.NET will call them in step 5.

## Check the migration tool

You will run migration commands after adding all API classes in step 6. For now, check that the EF Core tool is installed:

```powershell
dotnet ef --version
```

If the command is missing, install it:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.12
```

## How the migration files are created

In [step 6](web-api-example.md#try-the-api), from inside your **new** `StoreApi` project, run these commands in this order:

```powershell
dotnet ef migrations add InitialStore --output-dir Data/Migrations
dotnet ef migrations add ProductProcedures --output-dir Data/Migrations
```

The first command generates the `Products` and Identity table migration. The second generates an empty migration for your procedure SQL. EF Core creates these files automatically:

```text
Data/Migrations/
  <timestamp>_InitialStore.cs
  <timestamp>_InitialStore.Designer.cs
  <timestamp>_ProductProcedures.cs
  <timestamp>_ProductProcedures.Designer.cs
  StoreDbContextModelSnapshot.cs
```

You do not manually create the migration class, Designer files, or model snapshot. Once generated, open `<timestamp>_ProductProcedures.cs` and put the procedure SQL in its `Up` and `Down` methods.

## Procedure SQL for the generated migration

This is the complete `ProductProcedures` migration. Keep the generated filename and replace the contents of that file with this code. `Up` creates both procedures. `Down` removes them if you roll back the migration.

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProductProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SQL Server requires CREATE PROCEDURE to start its own batch.
            migrationBuilder.Sql("""
                EXEC(N'CREATE OR ALTER PROCEDURE dbo.Products_SearchByMinimumPrice
                    @MinimumPrice decimal(10,2)
                AS
                BEGIN
                    SET NOCOUNT ON;
                    SELECT ProductId, Name, Price, CreatedUtc
                    FROM dbo.Products
                    WHERE Price >= @MinimumPrice
                    ORDER BY Price, ProductId;
                END');
                """);

            migrationBuilder.Sql("""
                EXEC(N'CREATE OR ALTER PROCEDURE dbo.Products_Create
                    @Name nvarchar(100),
                    @Price decimal(10,2),
                    @ProductId int OUTPUT
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO dbo.Products (Name, Price) VALUES (@Name, @Price);
                    SET @ProductId = CONVERT(int, SCOPE_IDENTITY());
                END');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.Products_Create;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.Products_SearchByMinimumPrice;");
        }
    }
}
```

The `EXEC(N'...')` wrapper lets SQL Server run `CREATE OR ALTER PROCEDURE` as its own batch. `@MinimumPrice` is an input parameter. `@ProductId OUTPUT` returns the ID created by the insert.

## Apply migrations automatically when the API starts

The `Program.cs` shown in step 3 registers `StoreDbContext`, then calls `Database.MigrateAsync()` in Development:

```csharp
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<StoreDbContext>()
        .Database.MigrateAsync();
}
```

The [final walkthrough](web-api-example.md#try-the-api) shows how to start the API. On the first start, EF Core creates `StoreInterviewDb`, tables, and procedures. On later starts, it reads `__EFMigrationsHistory` and applies only pending migrations. This automatic application is configured for Development only.

If you want to apply migrations **without starting the API**, run `dotnet ef database update` inside your `StoreApi` project.

## Check that SQL Server created the procedures

List the two procedures:

```powershell
sqlcmd -S '(localdb)\AdoNetInterview' -E -d StoreInterviewDb `
    -Q "SELECT name FROM sys.procedures WHERE name LIKE 'Products_%' ORDER BY name"
```

You should see `Products_Create` and `Products_SearchByMinimumPrice`. After inserting a product through the API, try the search directly:

```powershell
sqlcmd -S '(localdb)\AdoNetInterview' -E -d StoreInterviewDb `
    -Q "EXEC dbo.Products_SearchByMinimumPrice @MinimumPrice = 1000"
```

## Change a procedure later

Run `dotnet ef migrations add UpdateProductProcedure` from your project. EF Core creates another empty migration file. Put the new `CREATE OR ALTER PROCEDURE` definition in `Up`, and the previous definition in `Down`. Apply it with `dotnet ef database update` or start the API in Development.

**Next:** [Call the procedures with ADO.NET](ado-net-methods.md).
