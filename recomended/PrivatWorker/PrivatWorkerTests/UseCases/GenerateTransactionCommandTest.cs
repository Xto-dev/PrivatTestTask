using System.Linq.Expressions;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PrivatWorker.Entities;
using PrivatWorker.UseCases;

namespace PrivatWorkerTests.UseCases;

[Trait("Category", "Unit")]
public class GenerateTransactionCommandTest
{
    private static readonly Expression<Func<ITransactionRepository, Task>> AddTransactionExpression =
        a => a.AddTransactionAsync(It.IsAny<Transaction>(), It.IsAny<CancellationToken>());

    private readonly Mock<ITransactionLog> _log;
    private readonly Mock<ITransactionRepository> _repository;
    private readonly Mock<ITransactionServices> _services;
    private readonly GenerateTransactionCommand _command;

    public GenerateTransactionCommandTest()
    {
        var fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        _log = fixture.Freeze<Mock<ITransactionLog>>();
        _repository = fixture.Freeze<Mock<ITransactionRepository>>();
        _services = fixture.Freeze<Mock<ITransactionServices>>();

        _command = fixture.Create<GenerateTransactionCommand>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAddTransactionAsyncOnce()
    {
        await _command.ExecuteAsync();

        _repository.Verify(AddTransactionExpression, Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogTransactionCreatedOnce()
    {
        await _command.ExecuteAsync();

        _log.Verify(a => a.TransactionCreated(It.IsAny<Transaction>()), Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassTransactionWithId()
    {
        var actualId = Guid.Empty;
        _repository
            .Setup(AddTransactionExpression)
            .Callback((Transaction t, CancellationToken _) => actualId = t.Id);

        var expectedId = Guid.Parse("8FF1D35A-FC08-4C9F-B7FF-CFA0F480F66E");
        _services.Setup(a => a.NewGuid()).Returns(expectedId);

        await _command.ExecuteAsync();

        Assert.Equal(expectedId, actualId);
    }

    [Theory]
    [InlineData(0.0, 0.00)]
    [InlineData(0.5, 5000.00)]
    [InlineData(1.0, 10000.00)]
    public async Task ExecuteAsync_ShouldPassTransactionWithAmount(double nextDouble, double expectedAmount)
    {
        var actualAmount = 0M;
        _repository
            .Setup(AddTransactionExpression)
            .Callback((Transaction t, CancellationToken _) => actualAmount = t.Amount);

        _services.Setup(a => a.NextDouble()).Returns(nextDouble);

        await _command.ExecuteAsync();

        Assert.Equal((decimal)expectedAmount, actualAmount);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassTransactionWithDate()
    {
        var actualDate = DateOnly.MinValue;
        _repository
            .Setup(AddTransactionExpression)
            .Callback((Transaction t, CancellationToken _) => actualDate = t.Date);

        var expectedDate = new DateOnly(2026, 01, 09);
        _services.Setup(a => a.Today()).Returns(expectedDate);

        await _command.ExecuteAsync();

        Assert.Equal(expectedDate, actualDate);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassTransactionWithClientId()
    {
        var actualClientId = 0;
        _repository
            .Setup(AddTransactionExpression)
            .Callback((Transaction t, CancellationToken _) => actualClientId = t.Message.ClientId);

        var expectedClientId = 536;
        var minInclusive = 1;
        var maxInclusive = 50001;
        _services.Setup(a => a.NextInt(minInclusive, maxInclusive)).Returns(expectedClientId);

        await _command.ExecuteAsync();

        Assert.Equal(expectedClientId, actualClientId);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassTransactionWithAccountNumber()
    {
        var actualAccountNumber = string.Empty;
        _repository
            .Setup (AddTransactionExpression)
            .Callback((Transaction t, CancellationToken _) => actualAccountNumber = t.Message.AccountNumber);

        var expectedNumber = 1234567890;
        var minInclusive = 0;
        var maxInclusive = 1_000_000_000;
        _services.Setup(a => a.NextInt(minInclusive, maxInclusive)).Returns(expectedNumber);
        var expectedAccountNumber = "UA" + expectedNumber.ToString("D10");

        await _command.ExecuteAsync();

        Assert.Equal(expectedAccountNumber, actualAccountNumber);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassTransactionWithOperationTypeOffline()
    {
        var actualOperationType = OperationType.online;
        _repository
            .Setup (AddTransactionExpression)
            .Callback((Transaction t, CancellationToken _) => actualOperationType = t.Message.OperationType);

        double randomDouble = 0.3;
        _services.Setup(a => a.NextDouble()).Returns(randomDouble);
        var expectedOperationType = OperationType.offline;

        await _command.ExecuteAsync();

        Assert.Equal(expectedOperationType, actualOperationType);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassTransactionWithOperationTypeOnline()
    {
        var actualOperationType = OperationType.online;
        _repository
            .Setup(AddTransactionExpression)
            .Callback((Transaction t, CancellationToken _) => actualOperationType = t.Message.OperationType);

        double randomDouble = 0.6;
        _services.Setup(a => a.NextDouble()).Returns(randomDouble);
        var expectedOperationType = OperationType.online;

        await _command.ExecuteAsync();

        Assert.Equal(expectedOperationType, actualOperationType);
    }
}