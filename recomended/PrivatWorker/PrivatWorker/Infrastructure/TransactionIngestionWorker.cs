using PrivatWorker.UseCases;

namespace PrivatWorker.Infrastructure;

public class TransactionIngestionWorker(
    IWorkerLog log,
    GenerateTransactionCommand command
) : BackgroundService
{
    private const int TimerIntervalInSeconds = 5;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(TimerIntervalInSeconds));
            while (true)
            {
                await timer.WaitForNextTickAsync(cancellationToken);
                await command.ExecuteAsync(cancellationToken);
            }
        }
        catch (Exception exception)
        {
            log.ExecuteAsyncException(exception);
        }
    }
}