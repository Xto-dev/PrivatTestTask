using PrivatWorker.Model.Transactions;

namespace PrivatWorker.Application.Transactions
{
    public interface ITransactionRepository
    {
        public Task AddAsync(CancellationToken cancelToken, Transaction transaction);
        public Task<int> ChangeStatusByParityAsync(CancellationToken cancelToken, bool parity);
    }
}
