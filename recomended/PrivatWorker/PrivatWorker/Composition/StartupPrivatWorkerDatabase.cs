using PrivatWorker.Entities;
using PrivatWorker.Infrastructure;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

public static class StartupPrivatWorkerDatabase
{
    public static void AddPrivatWorkerDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var connectionString = configuration.GetConnectionString("Default");
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddTransient<ITransactionRepository>(_ => new TransactionRepository(connectionString));
    }
}