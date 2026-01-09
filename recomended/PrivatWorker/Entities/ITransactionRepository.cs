namespace PrivatWorker.Entities
{
    public interface ITransactionRepository
    {
        public Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
        public Task<int> ChangeTransactionsStatusByParityAsync(bool parity, CancellationToken cancellationToken);
    }
}
