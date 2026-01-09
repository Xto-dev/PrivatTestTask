namespace PrivatWorker.Entities;

public class Transaction
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal Amount { get; set; }
    public int Status { get; set; }
    public required TransactionMessage Message { get; set; }
}