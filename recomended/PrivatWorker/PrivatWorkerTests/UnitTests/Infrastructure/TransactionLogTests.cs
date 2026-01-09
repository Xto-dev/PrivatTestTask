using Microsoft.Extensions.Logging;
using Moq;
using PrivatWorker.Entities;
using PrivatWorker.Infrastructure;

namespace PrivatWorkerTests.UnitTests.Infrastructure;

public class TransactionLogTests
{
    [Fact]
    public void ExecuteAsyncException_OutputLogAsync()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TransactionLog>>();
        var log = new TransactionLog(mockLogger.Object);
        var exception = new InvalidOperationException("Test exception");

        // Act
        log.ExecuteAsyncException(exception);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains("Execute async failed.")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Fact]
    public void TransactionCreated_LogsInformationWithTransactionId()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TransactionLog>>();
        var log = new TransactionLog(mockLogger.Object);
        var id = Guid.NewGuid();
        var message = new TransactionMessage { OperationType = OperationType.offline };
        var transaction = new Transaction { Id = id, Message = message };

        // Act
        log.TransactionCreated(transaction);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Transaction created. Transaction id: {id}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }

    [Fact]
    public void TransactionsStatusChanged_LogsInformationWithIsParityAndRows()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<TransactionLog>>();
        var log = new TransactionLog(mockLogger.Object);
        bool isParity = false;
        int rows = 3;

        // Act
        log.TransactionsStatusChanged(isParity, rows);

        // Assert
        mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((o, t) => o.ToString()!.Contains($"Transaction status changed. isParity: {isParity}. rows: {rows}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once
        );
    }
}