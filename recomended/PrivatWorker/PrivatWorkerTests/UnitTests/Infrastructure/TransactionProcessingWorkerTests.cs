using Moq;
using PrivatWorker.Entities;
using PrivatWorker.Infrastructure;

namespace PrivatWorkerTests.UnitTests.Infrastructure;

public class TransactionProcessingWorkerTests
{
    [Fact]
    public async Task UpdateTransactionStatus_CallsRepositoryAndLogger()
    {
        // Arrange
        var mockLog = new Mock<ITransactionLog>();
        var mockRepo = new Mock<ITransactionRepository>();
        var worker = new TransactionProcessingWorker(mockLog.Object, mockRepo.Object);
        int rows = 3;

        var cancellationToken = CancellationToken.None;

        mockRepo.Setup(r => r.ChangeTransactionsStatusByParityAsync(It.IsAny<bool>(), CancellationToken.None))
                .ReturnsAsync(rows);

        // Act

        await worker.UpdateTransactionStatus(cancellationToken);

        // Assert

        mockRepo.Verify(
            r => r.ChangeTransactionsStatusByParityAsync(It.IsAny<bool>(), cancellationToken),
            Times.Once
        );

        mockLog.Verify(l => l.TransactionsStatusChanged(It.IsAny<bool>(), rows), Times.Once);
    }

    [Fact]
    public async Task UpdateTransactionStatus_WhenCancelled_ThrowsOperationCanceledException()
    {
        // Arrange
        var mockLog = new Mock<ITransactionLog>();
        var mockRepo = new Mock<ITransactionRepository>();
        var worker = new TransactionProcessingWorker(mockLog.Object, mockRepo.Object);
        var cancellationToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            worker.UpdateTransactionStatus(cancellationToken));
    }
}