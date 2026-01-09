using PrivatWorker.Entities;

namespace PrivatWorker.Infrastructure
{
    public class TransactionIngestionWorker(
        ITransactionLog log,
        ITransactionRepository repository
    ) : BackgroundService
    {
        const int TimerIntervalInSeconds = 5;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(TimerIntervalInSeconds));
                while (true)
                {
                    await timer.WaitForNextTickAsync(stoppingToken);
                    await CreateTransaction(stoppingToken);
                }
            }
            catch (Exception exception)
            {
                log.ExecuteAsyncException(exception);
            }
        }
        public async Task CreateTransaction(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var transaction = GenerateTransaction();
            await repository.AddTransactionAsync(transaction, stoppingToken);
            log.TransactionCreated(transaction);
        }

        static Transaction GenerateTransaction()
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
                    OperationType = Random.Shared.NextDouble() < 0.5 ? OperationType.Online : OperationType.Offline
                }
            };
        }
    }
}
