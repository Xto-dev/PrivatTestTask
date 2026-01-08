using PrivatWorker.Model.Transactions;

namespace PrivatWorker.Application.Transactions
{
    public class CreateTransaction
    {
        private readonly ITransactionLogger _logger;
        private readonly ITransactionRepository _repository;

        public CreateTransaction(ITransactionLogger logger, ITransactionRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        public async Task ExecuteAsync(CancellationToken cancelToken, Transaction transaction)
        {
            await _repository.AddAsync(cancelToken, transaction);
            _logger.TransactionCreated(transaction.TransactionId, transaction.Message.ClientId);
        }
    }
}
