using PrivatWorker.Application.Transactions;
using PrivatWorker.Infra.Logging;
using PrivatWorker.Model.Transactions;

namespace PrivatWorker.Workers
{

    public class TransactionIngestionWorker : BackgroundService
    {
        private readonly CreateTransaction _createTransaction;
        private readonly ErrorLogger _errorLogger;

        public TransactionIngestionWorker(CreateTransaction createTransaction, ErrorLogger errorLogger)
        {
            _createTransaction = createTransaction;
            _errorLogger = errorLogger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
                while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
                {
                    Transaction transaction = new Transaction
                    {
                        Amount = (decimal)Math.Round(Random.Shared.NextDouble() * 10000, 2),
                        Date = DateOnly.FromDateTime(DateTime.Today),
                        TransactionId = Guid.NewGuid(),
                        Message = new TransactionMessage
                        {
                            AccountNumber = "UA" + Random.Shared.Next(1_000_000_000).ToString("D10"),
                            ClientId = Random.Shared.Next(1, 50001),
                            Type = Random.Shared.NextDouble() < 0.5 ? OperationType.Online : OperationType.Offline
                        }
                    };

                    await _createTransaction.ExecuteAsync(stoppingToken, transaction);
                }
            }
            catch (Exception exception)
            {
                _errorLogger.LogError("An error occurred while ingesting transaction.", exception);
            }
        }
    }
}
