using Npgsql;
using NpgsqlTypes;
using PrivatWorker.Application.Transactions;
using PrivatWorker.Model.Transactions;
using System.Text.Json;

namespace PrivatWorker.Infra.Persistence
{
    internal class TransactionRepository : ITransactionRepository
    {
        private readonly string _connectionString;
        public TransactionRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("PostgreSql");
        }

        public async Task<int> ChangeStatusByParityAsync(CancellationToken cancelToken, bool parity)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(cancelToken);

            var sql = parity
                ? "UPDATE t1 SET status = 1 WHERE status = 0 AND id % 2 = 0"
                : "UPDATE t1 SET status = 1 WHERE status = 0 AND id % 2 = 1";

            await using var cmd = new NpgsqlCommand(sql, conn);
            int rows = await cmd.ExecuteNonQueryAsync(cancelToken);

            return rows;
        }

        public async Task AddAsync(CancellationToken cancelToken, Transaction transaction)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(cancelToken);

            var message = MapMessageToJson(transaction.Message);

            await using var cmd = new NpgsqlCommand(@"
            INSERT INTO t1 (operation_date, amount, status, operation_guid, message)
            VALUES (@date, @amount, 0, @guid, @message)", conn);

            cmd.Parameters.AddWithValue("date", transaction.Date);
            cmd.Parameters.AddWithValue("amount", transaction.Amount);
            cmd.Parameters.AddWithValue("guid", transaction.TransactionId);
            cmd.Parameters.Add(new NpgsqlParameter("message", NpgsqlDbType.Jsonb) { Value = message });

            await cmd.ExecuteNonQueryAsync(cancelToken);
        }

        private string MapMessageToJson(TransactionMessage message)
        {
            return JsonSerializer.Serialize(new
            {
                account_number = message.AccountNumber,
                client_id = message.ClientId,
                operation_type = message.Type.ToString()
            });
        }
    }
}
