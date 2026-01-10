namespace PrivatWorker.UseCases;

public interface ITransactionServices
{
    double NextDouble();
    int NextInt(int minInclusive, int maxExclusive);
    Guid NewGuid();
    DateOnly Today();
    int NowSecond();
}