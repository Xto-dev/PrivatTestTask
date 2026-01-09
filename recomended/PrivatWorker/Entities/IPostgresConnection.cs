using Npgsql;

namespace PrivatWorker.Entities
{
    public interface IPostgresConnection : IDisposable, IAsyncDisposable
    {
        Task OpenAsync(CancellationToken ct = default);
        NpgsqlCommand CreateCommand();
    }
}
