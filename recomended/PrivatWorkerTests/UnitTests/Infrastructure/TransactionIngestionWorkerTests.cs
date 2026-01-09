using Moq;
using PrivatWorker.Entities;
using PrivatWorker.Infrastructure;

namespace PrivatWorkerTests.UnitTests.Infrastructure
{
    public class TransactionIngestionWorkerTests
    {
        [Fact]
        public async Task CreateTransaction_CallsRepositoryAndLoggerWithValidTransaction()
        {
            // Arrange
            var mockLog = new Mock<ITransactionLog>();
            var mockRepo = new Mock<ITransactionRepository>();
            var worker = new TransactionIngestionWorker(mockLog.Object, mockRepo.Object);

            var cancellationToken = CancellationToken.None;

            // Act
            await worker.CreateTransaction(cancellationToken);

            // Assert
            mockRepo.Verify(
                r => r.AddTransactionAsync(It.IsAny<Transaction>(), cancellationToken),
                Times.Once
            );
            mockLog.Verify(
                l => l.TransactionCreated(It.IsAny<Transaction>()),
                Times.Once
            );
            mockRepo.Verify(r => r.AddTransactionAsync(
                It.Is<Transaction>(t =>
                    t.Id != Guid.Empty &&
                    t.Amount > 0 && t.Amount <= 10000 &&
                    t.Date == DateOnly.FromDateTime(DateTime.Today) &&
                    t.Message != null &&
                    !string.IsNullOrEmpty(t.Message.AccountNumber) &&
                    (t.Message.ClientId >= 1 && t.Message.ClientId <= 50000)
                ),
                cancellationToken
            ), Times.Once);
        }

        [Fact]
        public async Task CreateTransaction_WhenCancelled_ThrowsOperationCanceledException()
        {
            //Arrange
            var mockLog = new Mock<ITransactionLog>();
            var mockRepo = new Mock<ITransactionRepository>();
            var worker = new TransactionIngestionWorker(mockLog.Object, mockRepo.Object);
            var cancellationToken = new CancellationToken(canceled: true);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                worker.CreateTransaction(cancellationToken));
        }
    }
}
