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
