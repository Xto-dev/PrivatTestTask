using Moq;
using PrivatWorker.Entities;
using PrivatWorker.Infrastructure;

namespace PrivatWorkerTests.UnitTests.Infrastructure
{
    public class TransactionRepositoryTests
    {
        [Fact]
        public async Task AddTransactionAsync_ShouldExecuteInsertCommand()
        {
            // Arrange
            var mockConnection = new Mock<IPostgresConnection>();
            var mockCommand = new Mock<Npgsql.NpgsqlCommand>();
            var mockFactory = new Mock<IDbConnectionFactory>();

            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            mockConnection.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>()))
                          .Returns(Task.CompletedTask);
            mockConnection.Setup(c => c.CreateCommand())
                          .Returns(mockCommand.Object);

            mockCommand.Setup(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(1);

            var repository = new TransactionRepository(mockFactory.Object);

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 100.5m,
                Message = new TransactionMessage
                {
                    AccountNumber = "UA1234567890",
                    ClientId = 1,
                    OperationType = OperationType.Offline
                }
            };

            // Act
            await repository.AddTransactionAsync(transaction, CancellationToken.None);

            // Assert
            mockConnection.Verify(c => c.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockCommand.VerifySet(cmd => cmd.CommandText = It.IsAny<string>(), Times.Once);
            mockCommand.Verify(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddTransactionAsync_WhenTransactionIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var mockFactory = new Mock<IDbConnectionFactory>();
            var repository = new TransactionRepository(mockFactory.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                repository.AddTransactionAsync(null!, CancellationToken.None));
        }

        [Fact]
        public async Task AddTransactionAsync_WhenMessageIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            var mockFactory = new Mock<IDbConnectionFactory>();
            var repository = new TransactionRepository(mockFactory.Object);

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Date = DateOnly.FromDateTime(DateTime.Now),
                Amount = 100,
                Message = null!
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                repository.AddTransactionAsync(transaction, CancellationToken.None));
        }

        [Fact]
        public async Task ChangeTransactionsStatusByParityAsync_ShouldExecuteUpdateCommand()
        {
            // Arrange
            var mockConnection = new Mock<IPostgresConnection>();
            var mockCommand = new Mock<Npgsql.NpgsqlCommand>();
            var mockFactory = new Mock<IDbConnectionFactory>();

            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            mockConnection.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>()))
                          .Returns(Task.CompletedTask);
            mockConnection.Setup(c => c.CreateCommand())
                          .Returns(mockCommand.Object);

            mockCommand.Setup(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()))
                       .ReturnsAsync(1);

            var repository = new TransactionRepository(mockFactory.Object);

            bool isParity = true;

            // Act
            await repository.ChangeTransactionsStatusByParityAsync(isParity, CancellationToken.None);

            // Assert
            mockConnection.Verify(c => c.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockCommand.VerifySet(cmd => cmd.CommandText = It.IsAny<string>(), Times.Once);
            mockCommand.Verify(cmd => cmd.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
