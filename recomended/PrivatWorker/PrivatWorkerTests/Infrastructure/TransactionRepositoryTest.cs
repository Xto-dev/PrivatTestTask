using AutoFixture;
using PrivatWorker.Entities;
using PrivatWorker.Infrastructure;

namespace PrivatWorkerTests.Infrastructure;

[Trait("Category", "DB")]
[Trait("Category", "Integration")]
public class TransactionRepositoryTest : IAsyncLifetime
{
    private readonly Fixture _fixture = new();
    private readonly TestDatabase _testDatabase = new();
    private TransactionRepository _repository = null!;

    public async Task InitializeAsync()
    {
        await _testDatabase.InitializeAsync();
        _repository = new TransactionRepository(_testDatabase.ConnectionString);
    }

    public async Task DisposeAsync() => await _testDatabase.DisposeAsync();

    private Transaction CreateTestTransaction() => _fixture.Build<Transaction>()
        .With(x => x.Id, Guid.NewGuid())
        .With(x => x.Date, DateOnly.FromDateTime(DateTime.Today))
        .With(x => x.Amount, 100.50m)
        .With(x => x.Message, _fixture.Build<TransactionMessage>()
            .With(x => x.ClientId, 12345)
            .With(x => x.AccountNumber, "1234567890")
            .With(x => x.OperationType, OperationType.online)
            .Create())
        .Create();

    [Fact]
    public async Task AddTransactionAsync_WithValidTransaction_ShouldInsertTransaction()
    {
        await _testDatabase.ClearDatabaseAsync();
        var transaction = CreateTestTransaction();
        await _repository.AddTransactionAsync(transaction);

        var actual = await _testDatabase.GetTransactionCountByGuidAsync(transaction.Id);

        Assert.Equal(1, actual);
    }

    [Fact]
    public async Task AddTransactionAsync_WithNullTransaction_ShouldThrowArgumentNullException() =>
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _repository.AddTransactionAsync(null!));

    [Fact]
    public async Task AddTransactionAsync_WithNullMessage_ShouldThrowArgumentNullException()
    {
        var transaction = CreateTestTransaction();
        transaction.Message = null!;

        await Assert.ThrowsAsync<ArgumentNullException>(
            () => _repository.AddTransactionAsync(transaction));
    }

    [Fact]
    public async Task AddTransactionAsync_InsertsCorrectTransactionData()
    {
        await _testDatabase.ClearDatabaseAsync();
        var transaction = CreateTestTransaction();
        await _repository.AddTransactionAsync(transaction);
        
        var (operationDate, amount, status, operationGuid, messageJson) =
            await _testDatabase.GetTransactionByGuidAsync(transaction.Id);
        
        Assert.Equal(transaction.Date, operationDate);
        Assert.Equal(transaction.Amount, amount);
        Assert.Equal(0, status);
        Assert.Equal(transaction.Id, operationGuid);
        Assert.Contains("account_number", messageJson);
        Assert.Contains("1234567890", messageJson);
    }

    [Fact]
    public async Task ChangeTransactionsStatusByParityAsync_WithEvenParity_ShouldUpdateEvenIdTransactions()
    {
        await _testDatabase.ClearDatabaseAsync();
        await _testDatabase.InsertTestTransactionsAsync(6);
        const bool parity = true;
        
        var actualAffecterRowsCount = await _repository.ChangeTransactionsStatusByParityAsync(parity);
        
        var actualEvenCount = await _testDatabase.GetTransactionCountByStatusAndParityAsync(1, 1);
        var actualOddCount = await _testDatabase.GetTransactionCountByStatusAndParityAsync(0, 0);
        Assert.Equal(3, actualAffecterRowsCount);
        Assert.Equal(3, actualEvenCount);
        Assert.Equal(3, actualOddCount);
    }

    [Fact]
    public async Task ChangeTransactionsStatusByParityAsync_WithOddParity_ShouldUpdateOddIdTransactions()
    {
        await _testDatabase.ClearDatabaseAsync();
        await _testDatabase.InsertTestTransactionsAsync(6);
        const bool parity = false;
        
        var actualAffectedRows = await _repository.ChangeTransactionsStatusByParityAsync(parity);
        
        Assert.Equal(3, actualAffectedRows);
        
        var actualEvenCount = await _testDatabase.GetTransactionCountByStatusAndParityAsync(1, 0);
        var actualOddCount = await _testDatabase.GetTransactionCountByStatusAndParityAsync(0, 1);
        Assert.Equal(3, actualEvenCount);
        Assert.Equal(3, actualOddCount);
    }

    [Fact]
    public async Task ChangeTransactionsStatusByParityAsync_WithNoMatchingRecords_ShouldReturnZero()
    {
        await _testDatabase.ClearDatabaseAsync();
        const bool parity = true;
        
        var affectedRows = await _repository.ChangeTransactionsStatusByParityAsync(parity);
        
        Assert.Equal(0, affectedRows);
    }

    [Fact]
    public async Task ChangeTransactionsStatusByParityAsync_OnlyAffectsStatusZeroRecords()
    {
        await _testDatabase.ClearDatabaseAsync();
        await _testDatabase.InsertTestTransactionsWithMixedStatusAsync(4);
        const bool parity = true;
        
        var affectedRows = await _repository.ChangeTransactionsStatusByParityAsync(parity);
        
        var result = await _testDatabase.GetTransactionCountByStatusAsync(1);
        Assert.Equal(0, affectedRows);
        Assert.Equal(2, result);
    }

    [Fact]
    public async Task ChangeTransactionsStatusByParityAsync_RepeatCall_ShouldNotAffectAlreadyUpdatedRecords()
    {
        await _testDatabase.ClearDatabaseAsync();
        await _testDatabase.InsertTestTransactionsAsync(6);
        const bool parity = true;
        
        var actualFirstCall = await _repository.ChangeTransactionsStatusByParityAsync(parity);
        var actualSecondCall = await _repository.ChangeTransactionsStatusByParityAsync(parity);
        
        Assert.Equal(3, actualFirstCall);
        Assert.Equal(0, actualSecondCall);
    }
}