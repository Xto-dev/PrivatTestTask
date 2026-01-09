using PrivatWorker.Entities;

namespace PrivatWorker.Infrastructure;

public class TransactionIngestionWorker(
    ITransactionLog log,
    ITransactionRepository repository
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
                await CreateTransaction(cancellationToken);
            }
        }
        catch (Exception exception)
        {
            log.ExecuteAsyncException(exception);
        }
    }

    public async Task CreateTransaction(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var transaction = GenerateTransaction();
        await repository.AddTransactionAsync(transaction, cancellationToken);
        log.TransactionCreated(transaction);
    }

    private static Transaction GenerateTransaction()
    {
        return new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = (decimal)Math.Round(Random.Shared.NextDouble() * 10000, 2),
            Date = DateOnly.FromDateTime(DateTime.Today),
            Message = new TransactionMessage
            {
                AccountNumber = "UA" + Random.Shared.Next(1_000_000_000).ToString("D10"),
                ClientId = Random.Shared.Next(1, 50001),
                OperationType = Random.Shared.NextDouble() < 0.5 ? OperationType.online : OperationType.offline
            }
        };
    }
}