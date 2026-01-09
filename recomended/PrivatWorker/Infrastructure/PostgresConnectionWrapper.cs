using Npgsql;
using PrivatWorker.Entities;

namespace PrivatWorker.Infrastructure
{
    public class PostgresConnectionWrapper : IPostgresConnection
    {
        private readonly NpgsqlConnection _inner;

        public PostgresConnectionWrapper(string connectionString)
        {
            _inner = new NpgsqlConnection(connectionString);
        }

        public async Task OpenAsync(CancellationToken ct = default)
            => await _inner.OpenAsync(ct);

        public NpgsqlCommand CreateCommand()
            => _inner.CreateCommand();

        public void Dispose() => _inner.Dispose();

        public ValueTask DisposeAsync() => _inner.DisposeAsync();
    }
}
