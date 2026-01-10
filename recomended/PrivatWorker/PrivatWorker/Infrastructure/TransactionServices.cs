using PrivatWorker.UseCases;

namespace PrivatWorker.Infrastructure;

public class TransactionServices : ITransactionServices
{
    private readonly Random _random = new();
    public double NextDouble() => _random.NextDouble();

    public int NextInt(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);

    public Guid NewGuid() => Guid.NewGuid();

    public DateOnly Today() => DateOnly.FromDateTime(DateTime.Today);

    public int NowSecond() => DateTime.Now.Second;
}