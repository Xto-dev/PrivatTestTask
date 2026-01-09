using PrivatWorker.Entities;

namespace PrivatWorker.Infrastructure
{
    public class TransactionProcessingWorker(
        ITransactionLog log,
        ITransactionRepository repository
    ) : BackgroundService
    {
        const int TimerIntervalInSeconds = 3;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(TimerIntervalInSeconds));
                while (true)
                {
                    await timer.WaitForNextTickAsync(stoppingToken);
                    await UpdateTransactionStatus(stoppingToken);
                }
            }
            catch (Exception exception)
            {
                log.ExecuteAsyncException(exception);
            }
        }

        public async Task UpdateTransactionStatus(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var now = DateTime.Now;
            var isParity = now.Second % 2 == 0;
            var rows = await repository.ChangeTransactionsStatusByParityAsync(isParity, stoppingToken);
            log.TransactionsStatusChanged(isParity, rows);
        }
    }
}
