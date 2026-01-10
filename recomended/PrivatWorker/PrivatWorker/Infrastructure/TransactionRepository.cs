using Npgsql;
using NpgsqlTypes;
using PrivatWorker.Entities;
using PrivatWorker.UseCases;
using System.Data;
using System.Text.Json;

namespace PrivatWorker.Infrastructure;

public class TransactionRepository(
    string connectionString
) : ITransactionRepository
{
    public async Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(transaction.Message);

        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);

        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
                          INSERT INTO t1 (operation_date, amount, status, operation_guid, message)
                          VALUES (@date, @amount, 0, @guid, @message)
                          """;
        cmd.CommandType = CommandType.Text;
        cmd.Parameters.AddWithValue("date", transaction.Date);
        cmd.Parameters.AddWithValue("amount", transaction.Amount);
        cmd.Parameters.AddWithValue("guid", transaction.Id);
        var message = MapMessageToJson(transaction.Message);
        cmd.Parameters.Add(new NpgsqlParameter("message", NpgsqlDbType.Jsonb) { Value = message });
        await cmd.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<int> ChangeTransactionsStatusByParityAsync(bool parity, CancellationToken cancellationToken = default)
    {
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(cancellationToken);
        var parityValue = parity ? 1 : 0;
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = $"UPDATE t1 SET status = 1 WHERE status = 0 AND id % 2 = {parityValue}";
        cmd.CommandType = CommandType.Text;
        return await cmd.ExecuteNonQueryAsync(cancellationToken);
    }
    private static string MapMessageToJson(TransactionMessage message)
    {
        return JsonSerializer.Serialize(new
        {
            account_number = message.AccountNumber,
            client_id = message.ClientId,
            operation_type = message.OperationType.ToString("G")
        });
    }
}