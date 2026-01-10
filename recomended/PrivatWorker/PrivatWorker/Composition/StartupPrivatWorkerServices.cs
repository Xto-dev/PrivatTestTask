using PrivatWorker.UseCases;
using PrivatWorker.Infrastructure;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130

public static class StartupPrivatWorkerServices
{
    public static void AddPrivatWorkerServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<GenerateTransactionCommand>();
        services.AddTransient<UpdateTransactionStatusCommand>();
        services.AddHostedService<TransactionIngestionWorker>();
        services.AddHostedService<TransactionProcessingWorker>();
        services.AddTransient<ITransactionLog, TransactionLog>();
        services.AddTransient<ITransactionServices, TransactionServices>();
        services.AddTransient<IWorkerLog, WorkerLog>();
    }
}