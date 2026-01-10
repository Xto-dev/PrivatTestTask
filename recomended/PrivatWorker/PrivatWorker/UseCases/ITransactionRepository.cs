using PrivatWorker.Entities;

namespace PrivatWorker.UseCases;

public interface ITransactionRepository
{
    public Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken = default);
    public Task<int> ChangeTransactionsStatusByParityAsync(bool parity, CancellationToken cancellationToken = default);
}