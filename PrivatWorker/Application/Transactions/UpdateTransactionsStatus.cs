using System;
using System.Collections.Generic;
using System.Text;

namespace PrivatWorker.Application.Transactions
{
    public class UpdateTransactionsStatus
    {
        private readonly ITransactionRepository _repository;
        private readonly ITransactionLogger _logger;

        public UpdateTransactionsStatus(ITransactionRepository repository, ITransactionLogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancelToken, DateTime now)
        {
            bool isParity = now.Second % 2 == 0;

            int rows = await _repository.ChangeStatusByParityAsync(cancelToken, isParity);
            _logger.TransactionsStatusChanged(isParity, rows);
        }
    }
}
