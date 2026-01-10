using PrivatWorker.Entities;

namespace PrivatWorker.UseCases;

public interface ITransactionLog
{
    void ExecuteAsyncException(Exception exception);
    void TransactionCreated(Transaction transaction);
    void TransactionsStatusChanged(bool parity, int affectedRows);
}