using PrivatWorker.Entities;
using PrivatWorker.Infrastructure;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

public static class StartupPrivatWorkerServices
{
    public static void AddPrivatWorkerServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHostedService<TransactionIngestionWorker>();
        services.AddHostedService<TransactionProcessingWorker>();
        services.AddTransient<ITransactionLog, TransactionLog>();
    }
}