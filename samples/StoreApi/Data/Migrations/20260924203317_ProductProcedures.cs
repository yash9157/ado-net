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
