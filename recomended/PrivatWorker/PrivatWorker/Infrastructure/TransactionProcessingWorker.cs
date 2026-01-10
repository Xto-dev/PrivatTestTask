using PrivatWorker.UseCases;

namespace PrivatWorker.Infrastructure;

public class TransactionProcessingWorker(
    IWorkerLog log,
    UpdateTransactionStatusCommand command
) : BackgroundService
{
    private const int TimerIntervalInSeconds = 3;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(TimerIntervalInSeconds));
            while (true)
            {
                await timer.WaitForNextTickAsync(stoppingToken);
                await command.ExecuteAsync(stoppingToken);
            }
        }
        catch (Exception exception)
        {
            log.ExecuteAsyncException(exception);
        }
    }
}