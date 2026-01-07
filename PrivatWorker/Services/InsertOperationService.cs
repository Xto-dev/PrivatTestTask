using Npgsql;
using NpgsqlTypes;
using System.Text.Json;

namespace PrivatWorker.Services
{
    public class InsertOperationService : BackgroundService
    {
        private readonly ILogger<InsertOperationService> _logger;
        private readonly string _connectionString;

        public InsertOperationService(IConfiguration config, ILogger<InsertOperationService> logger)
        {
            _logger = logger;
            _connectionString = config.GetConnectionString("Db");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                await InsertOperationAsync(stoppingToken);
            }
        }

        private async Task InsertOperationAsync(CancellationToken ct)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(ct);


            var amount = Math.Round(Random.Shared.NextDouble() * 10000, 2);
            var guid = Guid.NewGuid();
            var date = DateOnly.FromDateTime(DateTime.Today);
            var message = JsonSerializer.Serialize(new
            {
                account_number = "UA" + Random.Shared.Next(1_000_000_000).ToString("D10"),
                client_id = Random.Shared.Next(1, 50001),
                operation_type = Random.Shared.NextDouble() < 0.5 ? "online" : "offline"
            });

            await using var cmd = new NpgsqlCommand(@"
            INSERT INTO t1 (operation_date, amount, status, operation_guid, message)
            VALUES (@date, @amount, 0, @guid, @message)", conn);

            cmd.Parameters.AddWithValue("date", date);
            cmd.Parameters.AddWithValue("amount", amount);
            cmd.Parameters.AddWithValue("guid", guid);
            cmd.Parameters.Add(new NpgsqlParameter("message", NpgsqlDbType.Jsonb) { Value = message });

            await cmd.ExecuteNonQueryAsync(ct);
            _logger.LogInformation("Inserted operation {Guid}", guid);
        }
    }
}
