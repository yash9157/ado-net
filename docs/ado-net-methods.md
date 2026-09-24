# 5. Call stored procedures with ADO.NET

Here is the real example used by this API: search for products with `dbo.Products_SearchByMinimumPrice`, then create a product with `dbo.Products_Create`. EF Core Identity still handles users; these product calls use ADO.NET directly.

## The five steps in every call

1. Get a `SqlConnection` from the connection factory.
2. Create a `SqlCommand` with the procedure name and `CommandType.StoredProcedure`.
3. Add typed `SqlParameter` values. Never concatenate user input into SQL.
4. Open the connection and run the right execute method.
5. Read the result. `await using` closes the reader, command, and connection.

The [migration explained in step 4](stored-procedures.md) creates the procedures when the API starts in Development.

## Reuse connection creation

This small common class stores the connection string and creates a connection for each operation:

```csharp
using Microsoft.Data.SqlClient;

namespace StoreApi.Data;

public sealed class SqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        _connectionString = connectionString;
    }

    public SqlConnection CreateConnection() => new(_connectionString);
}
```

Each operation disposes its connection after use. ADO.NET connection pooling handles reuse behind the scenes.

Register the factory and repository in `Program.cs` so the controller can receive them:

```csharp
builder.Services.AddSingleton(new SqlConnectionFactory(connectionString));
builder.Services.AddScoped<IProductStoredProcedureRepository,
    ProductStoredProcedureRepository>();
```

## Example 1: read rows with `ExecuteReaderAsync`

The search procedure takes `@MinimumPrice` and returns product rows. This is the important part of `SearchByMinimumPriceAsync`:

```csharp
await using SqlConnection connection = connectionFactory.CreateConnection();
await using var command = new SqlCommand(
    "dbo.Products_SearchByMinimumPrice", connection)
{
    CommandType = CommandType.StoredProcedure
};

SqlParameter price = command.Parameters.Add("@MinimumPrice", SqlDbType.Decimal);
price.Precision = 10;
price.Scale = 2;
price.Value = minimumPrice;

await connection.OpenAsync(cancellationToken);
await using SqlDataReader reader =
    await command.ExecuteReaderAsync(cancellationToken);

while (await reader.ReadAsync(cancellationToken))
{
    int id = reader.GetInt32(reader.GetOrdinal("ProductId"));
    string name = reader.GetString(reader.GetOrdinal("Name"));
    // Read Price and CreatedUtc the same way; see the complete method below.
}
```

Use `ExecuteReaderAsync` when SQL returns rows. The complete method maps all four columns into `ProductResponse` and returns `IReadOnlyList<ProductResponse>`—a read-only view of the results.

## Example 2: insert and get an output ID

The create procedure takes `@Name` and `@Price`, inserts a row, and sets `@ProductId OUTPUT`:

```csharp
await using SqlConnection connection = connectionFactory.CreateConnection();
await using var command = new SqlCommand("dbo.Products_Create", connection)
{
    CommandType = CommandType.StoredProcedure
};

command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value = request.Name.Trim();
SqlParameter price = command.Parameters.Add("@Price", SqlDbType.Decimal);
price.Precision = 10;
price.Scale = 2;
price.Value = request.Price;

SqlParameter id = command.Parameters.Add("@ProductId", SqlDbType.Int);
id.Direction = ParameterDirection.Output;

await connection.OpenAsync(cancellationToken);
await command.ExecuteNonQueryAsync(cancellationToken);
return (int)id.Value;
```

Use `ExecuteNonQueryAsync` when the procedure does not return rows. Read the output parameter **after** the command finishes. The API returns that ID in its HTTP 201 response.

## Repository contract

The controller calls this interface. One method searches for products; the other inserts and returns the new ID.

```csharp
using StoreApi.Contracts;

namespace StoreApi.Repositories;

public interface IProductStoredProcedureRepository
{
    Task<IReadOnlyList<ProductResponse>> SearchByMinimumPriceAsync(
        decimal minimumPrice,
        CancellationToken cancellationToken = default);

    Task<int> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);
}
```

## Complete ADO.NET repository

This is the full working implementation behind the two examples above, including cancellation, timeouts, and mapping every result column:

```csharp
using System.Data;
using Microsoft.Data.SqlClient;
using StoreApi.Contracts;
using StoreApi.Data;

namespace StoreApi.Repositories;

public sealed class ProductStoredProcedureRepository(
    SqlConnectionFactory connectionFactory)
    : IProductStoredProcedureRepository
{
    public async Task<IReadOnlyList<ProductResponse>> SearchByMinimumPriceAsync(
        decimal minimumPrice,
        CancellationToken cancellationToken = default)
    {
        var products = new List<ProductResponse>();

        await using SqlConnection connection = connectionFactory.CreateConnection();
        await using var command = new SqlCommand(
            "dbo.Products_SearchByMinimumPrice",
            connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 30
        };

        SqlParameter minimumPriceParameter =
            command.Parameters.Add("@MinimumPrice", SqlDbType.Decimal);
        minimumPriceParameter.Precision = 10;
        minimumPriceParameter.Scale = 2;
        minimumPriceParameter.Value = minimumPrice;

        await connection.OpenAsync(cancellationToken);
        await using SqlDataReader reader =
            await command.ExecuteReaderAsync(cancellationToken);

        int productId = reader.GetOrdinal("ProductId");
        int name = reader.GetOrdinal("Name");
        int price = reader.GetOrdinal("Price");
        int createdUtc = reader.GetOrdinal("CreatedUtc");

        while (await reader.ReadAsync(cancellationToken))
        {
            products.Add(new ProductResponse(
                reader.GetInt32(productId),
                reader.GetString(name),
                reader.GetDecimal(price),
                reader.GetDateTime(createdUtc)));
        }

        return products;
    }

    public async Task<int> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default)
    {
        await using SqlConnection connection = connectionFactory.CreateConnection();
        await using var command = new SqlCommand("dbo.Products_Create", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 30
        };

        command.Parameters.Add("@Name", SqlDbType.NVarChar, 100).Value =
            request.Name.Trim();

        SqlParameter priceParameter = command.Parameters.Add("@Price", SqlDbType.Decimal);
        priceParameter.Precision = 10;
        priceParameter.Scale = 2;
        priceParameter.Value = request.Price;

        SqlParameter idParameter = command.Parameters.Add("@ProductId", SqlDbType.Int);
        idParameter.Direction = ParameterDirection.Output;

        await connection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);

        return (int)idParameter.Value;
    }
}
```

## What each call does

| Part | Search | Create |
|---|---|---|
| `CommandType` | `StoredProcedure` | `StoredProcedure` |
| Parameters | `@MinimumPrice` input | `@Name` and `@Price` input, `@ProductId` output |
| Execute method | `ExecuteReaderAsync` | `ExecuteNonQueryAsync` |
| Result | Mapped product rows | New product ID |

For a query returning one value rather than rows, use `ExecuteScalarAsync` (for example, `SELECT COUNT(*) FROM dbo.Products`). That method is not needed by these two procedures. Keep user values in `SqlParameter` objects; do not join them into SQL strings.

## Where the API calls these methods

`GET /api/products?minimumPrice=1000` calls `SearchByMinimumPriceAsync`. `POST /api/products` calls `CreateAsync`. The controller contains no SQL; its complete source and the requests to test it are in [step 6](web-api-example.md).

**Next:** [Add the API endpoints and try them](web-api-example.md).
