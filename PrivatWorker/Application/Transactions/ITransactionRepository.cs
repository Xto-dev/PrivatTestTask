using PrivatWorker.Model.Transactions;
using System;
using System.Collections.Generic;
using System.Text;

namespace PrivatWorker.Application.Transactions
{
    public interface ITransactionRepository
    {
        public Task AddAsync(CancellationToken cancelToken, Transaction transaction);
        public Task<int> ChangeStatusByParityAsync(CancellationToken cancelToken, bool parity);
    }
}
