namespace PrivatWorker.Model.Transactions
{
    public class TransactionMessage
    {
        public int ClientId { get; set; }
        public string AccountNumber { get; set; } = "";
        public required OperationType Type { get; set; }
    }
}
