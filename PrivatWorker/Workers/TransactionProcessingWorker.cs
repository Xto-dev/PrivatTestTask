using PrivatWorker.Application.Transactions;
using PrivatWorker.Infra.Logging;

namespace PrivatWorker.Workers
{
    public class TransactionProcessingWorker : BackgroundService
    {
        private readonly UpdateTransactionsStatus _updateTransactionsStatus;
        private readonly ErrorLogger _errorLogger;
        public TransactionProcessingWorker(UpdateTransactionsStatus updateTransactionsStatus, ErrorLogger errorLogger)
        {
            _updateTransactionsStatus = updateTransactionsStatus;
            _errorLogger = errorLogger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(3));
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await _updateTransactionsStatus.ExecuteAsync(stoppingToken, DateTime.Now);
                }
            }
            catch (Exception exception)
            {
                _errorLogger.LogError("An error occurred while processing transactions.", exception);
            }
        }
    }
}
