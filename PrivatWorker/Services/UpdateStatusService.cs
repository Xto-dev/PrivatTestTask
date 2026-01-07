using Npgsql;
using System.Text.Json;

namespace PrivatWorker.Services
{
    public class UpdateStatusService : BackgroundService
    {
        private readonly ILogger<UpdateStatusService> _logger;
        private readonly string _connectionString;

        public UpdateStatusService(IConfiguration config, ILogger<UpdateStatusService> logger)
        {
            _connectionString = config.GetConnectionString("Db");
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                var isEvenSecond = DateTime.Now.Second % 2 == 0;
                await UpdateStatusAsync(isEvenSecond ? "even" : "odd", stoppingToken);
            }
        }

        private async Task UpdateStatusAsync(string parity, CancellationToken ct)
        {
            using var conn = new NpgsqlConnection(_connectionString);
            await conn.OpenAsync(ct);

            var sql = parity == "even"
                ? "UPDATE t1 SET status = 1 WHERE status = 0 AND id % 2 = 0"
                : "UPDATE t1 SET status = 1 WHERE status = 0 AND id % 2 = 1";

            await using var cmd = new NpgsqlCommand(sql, conn);
            var rows = await cmd.ExecuteNonQueryAsync(ct);
            _logger.LogInformation("Updated {Rows} {Parity} rows", rows, parity);
        }
    }
}
