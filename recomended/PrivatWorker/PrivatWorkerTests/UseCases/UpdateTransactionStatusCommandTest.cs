using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using PrivatWorker.UseCases;

namespace PrivatWorkerTests.UseCases;

[Trait("Category", "Unit")]
public class UpdateTransactionStatusCommandTest
{
    private readonly Mock<ITransactionLog> _log;
    private readonly Mock<ITransactionRepository> _repository;
    private readonly Mock<ITransactionServices> _services;

    private readonly UpdateTransactionStatusCommand _command;

    public UpdateTransactionStatusCommandTest()
    {
        var fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        _log = fixture.Freeze<Mock<ITransactionLog>>();
        _repository = fixture.Freeze<Mock<ITransactionRepository>>();
        _services = fixture.Freeze<Mock<ITransactionServices>>();

        _command = fixture.Create<UpdateTransactionStatusCommand>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAddTransactionAsyncOnce()
    {
        await _command.ExecuteAsync();

        _repository.Verify(
            a => a.ChangeTransactionsStatusByParityAsync(
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Fact]
    public async Task ExecuteAsync_ShouldLogTransactionsStatusChangedOnce()
    {
        await _command.ExecuteAsync();

        _log.Verify(
            a => a.TransactionsStatusChanged(
                It.IsAny<bool>(),
                It.IsAny<int>()),
            Times.Once());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(31)]
    [InlineData(59)]
    public async Task ExecuteAsync_ShouldPassIsParityFalseToRepository_WhenNowSecondOdd(int nowSecond)
    {
        _services.Setup(a => a.NowSecond()).Returns(nowSecond);

        await _command.ExecuteAsync();

        _repository.Verify(
            a => a.ChangeTransactionsStatusByParityAsync(
                false,
                It.IsAny<CancellationToken>()),
            Times.Once());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(30)]
    [InlineData(58)]
    public async Task ExecuteAsync_ShouldPassIsParityTrueToRepository_WhenNowSecondEven(int nowSecond)
    {
        _services.Setup(a => a.NowSecond()).Returns(nowSecond);

        await _command.ExecuteAsync();

        _repository.Verify(
            a => a.ChangeTransactionsStatusByParityAsync(
                true,
                It.IsAny<CancellationToken>()),
            Times.Once());
    }
}