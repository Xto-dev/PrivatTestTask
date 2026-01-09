namespace PrivatWorker.Entities
{
    public interface IDbConnectionFactory
    {
        IPostgresConnection CreateConnection();
    }
}
