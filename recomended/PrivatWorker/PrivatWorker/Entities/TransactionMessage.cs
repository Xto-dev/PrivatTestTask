namespace PrivatWorker.Entities;

public class TransactionMessage
{
    public int ClientId { get; set; }
    public string AccountNumber { get; set; } = "";
    public required OperationType OperationType { get; set; }
}