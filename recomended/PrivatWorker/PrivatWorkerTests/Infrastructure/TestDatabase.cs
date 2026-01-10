using Npgsql;
using NpgsqlTypes;
using Testcontainers.PostgreSql;

namespace PrivatWorkerTests.Infrastructure;

public class TestDatabase : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("postgres")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    public string ConnectionString { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        ConnectionString = _postgreSqlContainer.GetConnectionString();
        await CreateDatabaseSchemaAsync();
    }

    public async Task DisposeAsync() => await _postgreSqlContainer.DisposeAsync();

    private async Task CreateDatabaseSchemaAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
                              CREATE TABLE t1 (
                                  id SERIAL PRIMARY KEY,
                                  operation_date DATE NOT NULL,
                                  amount DECIMAL(19,4) NOT NULL,
                                  status INTEGER NOT NULL DEFAULT 0,
                                  operation_guid UUID NOT NULL,
                                  message JSONB NOT NULL)
                              """;
        await command.ExecuteNonQueryAsync();
    }

    public async Task ClearDatabaseAsync()
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = "TRUNCATE TABLE t1 RESTART IDENTITY";
        await command.ExecuteNonQueryAsync();
    }

    public async Task<int> InsertTestTransactionsAsync(int count)
    {
        var insertedIds = new List<int>();
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        for (var i = 0; i < count; i++)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                                  INSERT INTO t1 (operation_date, amount, status, operation_guid, message)
                                  VALUES (@date, @amount, 0, @guid, @message) RETURNING id
                                  """;
            command.Parameters.AddWithValue("date", DateOnly.FromDateTime(DateTime.Today));
            command.Parameters.AddWithValue("amount", 100.50m + i);
            command.Parameters.AddWithValue("guid", Guid.NewGuid());
            command.Parameters.Add(new NpgsqlParameter("message", NpgsqlDbType.Jsonb)
            {
                Value = "{\"account_number\":\"1234567890\",\"client_id\":12345,\"operation_type\":\"online\"}"
            });
            var result = await command.ExecuteScalarAsync();
            if (result != null)
                insertedIds.Add(Convert.ToInt32(result));
        }

        return insertedIds.Count;
    }

    public async Task<int> GetTransactionCountByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM t1 WHERE operation_guid = @guid";
        command.Parameters.AddWithValue("guid", guid);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<(DateOnly operationDate, decimal amount, int status, Guid operationGuid, string message)>
        GetTransactionByGuidAsync(Guid guid, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT operation_date, amount, status, operation_guid, message
                              FROM t1 WHERE operation_guid = @guid
                              """;
        command.Parameters.AddWithValue("guid", guid);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return (
            DateOnly.FromDateTime(reader.GetDateTime(0)),
            reader.GetDecimal(1),
            reader.GetInt32(2),
            reader.GetGuid(3),
            reader.GetString(4)
        );
    }

    public async Task<int> GetTransactionCountByStatusAndParityAsync(int status, int parity,
        CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM t1 WHERE status = @status AND id % 2 = @parity";
        command.Parameters.AddWithValue("status", status);
        command.Parameters.AddWithValue("parity", parity);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<int[]> InsertTestTransactionsWithMixedStatusAsync(int count,
        CancellationToken cancellationToken = default)
    {
        var insertedIds = new List<int>();
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        for (var i = 0; i < count; i++)
        {
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = """
                              INSERT INTO t1 (operation_date, amount, status, operation_guid, message)
                              VALUES (@date, @amount, @status, @guid, @message) RETURNING id
                              """;
            var status = i % 2 == 0 ? 1 : 0;
            cmd.Parameters.AddWithValue("date", DateOnly.FromDateTime(DateTime.Today));
            cmd.Parameters.AddWithValue("amount", 100.50m + i);
            cmd.Parameters.AddWithValue("status", status);
            cmd.Parameters.AddWithValue("guid", Guid.NewGuid());
            cmd.Parameters.Add(new NpgsqlParameter("message", NpgsqlDbType.Jsonb)
            {
                Value = "{\"account_number\":\"1234567890\",\"client_id\":12345,\"operation_type\":\"online\"}"
            });
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            if (result != null)
                insertedIds.Add(Convert.ToInt32(result));
        }

        return insertedIds.ToArray();
    }

    public async Task<int> GetTransactionCountByStatusAsync(int status, CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM t1 WHERE status = @status";
        command.Parameters.AddWithValue("status", status);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }
}