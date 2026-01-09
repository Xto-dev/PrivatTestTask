using PrivatWorker.Entities;

namespace PrivatWorker.Infrastructure
{
    public class PostgresConnectionFactory(
        string connectionString
        ) : IDbConnectionFactory
    {
        public IPostgresConnection CreateConnection() => new PostgresConnectionWrapper(connectionString);
    }
}
