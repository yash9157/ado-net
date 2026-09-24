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
