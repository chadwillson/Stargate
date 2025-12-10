using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Stargate.Repository;
using Stargate.Repository.Entities;
using Stargate.Repository.Interfaces;

namespace Stargate.IntegrationTests.Repositories
{
    [TestClass]
    public class LogRepositoryTests
    {
        private CustomWebApplicationFactory _factory = null!;
        private IServiceScope _scope = null!;
        private StargateContext _context = null!;
        private ILogRepository _repository = null!;

        [TestInitialize]
        public async Task Setup()
        {
            _factory = new CustomWebApplicationFactory();
            await _factory.ResetDatabaseAsync();
            _scope = _factory.Services.CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<StargateContext>();
            _repository = _scope.ServiceProvider.GetRequiredService<IUnitOfWork>().LogEntries;
        }

        [TestCleanup]
        public void Cleanup()
        {
            _scope?.Dispose();
            _factory?.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddLogEntryToDatabase()
        {
            // Arrange
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "Test",
                Message = "Test log message"
            };

            // Act
            await _repository.AddAsync(logEntry);
            await _context.SaveChangesAsync();

            // Assert
            logEntry.Id.Should().BeGreaterThan(0);
            var retrieved = await _repository.GetByIdAsync(logEntry.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Message.Should().Be("Test log message");
        }

        [TestMethod]
        public async Task AddAsync_WithException_ShouldStoreExceptionDetails()
        {
            // Arrange
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Category = "ErrorTest",
                Message = "An error occurred",
                Exception = "System.InvalidOperationException: Operation failed",
                StackTrace = "   at TestMethod() in Test.cs:line 42"
            };

            // Act
            await _repository.AddAsync(logEntry);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(logEntry.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Exception.Should().Contain("InvalidOperationException");
            retrieved.StackTrace.Should().Contain("Test.cs:line 42");
        }

        [TestMethod]
        public async Task AddAsync_WithCorrelationId_ShouldStoreCorrelationId()
        {
            // Arrange
            var correlationId = Guid.NewGuid().ToString();
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "CorrelationTest",
                Message = "Test message with correlation",
                CorrelationId = correlationId
            };

            // Act
            await _repository.AddAsync(logEntry);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(logEntry.Id);
            retrieved.Should().NotBeNull();
            retrieved!.CorrelationId.Should().Be(correlationId);
        }

        [TestMethod]
        public async Task AddAsync_WithRequestDetails_ShouldStoreHttpMetadata()
        {
            // Arrange
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "HttpRequest",
                Message = "GET /api/person responded 200",
                RequestPath = "/api/person",
                RequestMethod = "GET",
                StatusCode = 200,
                ElapsedMilliseconds = 150
            };

            // Act
            await _repository.AddAsync(logEntry);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(logEntry.Id);
            retrieved.Should().NotBeNull();
            retrieved!.RequestPath.Should().Be("/api/person");
            retrieved.RequestMethod.Should().Be("GET");
            retrieved.StatusCode.Should().Be(200);
            retrieved.ElapsedMilliseconds.Should().Be(150);
        }

        [TestMethod]
        public async Task AddAsync_WithAllFields_ShouldStoreCompleteLogEntry()
        {
            // Arrange
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Warning",
                Category = "CompleteTest",
                Message = "Complete log entry",
                Exception = "Test exception",
                StackTrace = "Test stack trace",
                Source = "TestMethod",
                CorrelationId = "test-correlation-id",
                UserId = "user123",
                RequestPath = "/api/test",
                RequestMethod = "POST",
                StatusCode = 400,
                ElapsedMilliseconds = 250,
                AdditionalData = "Extra information"
            };

            // Act
            await _repository.AddAsync(logEntry);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(logEntry.Id);
            retrieved.Should().NotBeNull();
            retrieved!.Level.Should().Be("Warning");
            retrieved.Category.Should().Be("CompleteTest");
            retrieved.Source.Should().Be("TestMethod");
            retrieved.UserId.Should().Be("user123");
            retrieved.AdditionalData.Should().Be("Extra information");
        }

        [TestMethod]
        public async Task GetAllAsync_ShouldReturnAllLogEntries()
        {
            // Arrange
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "Test1",
                Message = "Message 1"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Category = "Test2",
                Message = "Message 2"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Warning",
                Category = "Test3",
                Message = "Message 3"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().HaveCount(3);
        }

        [TestMethod]
        public async Task AddAsync_MultipleErrorLogs_ShouldStoreAllIndependently()
        {
            // Arrange
            var error1 = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Category = "ErrorTest",
                Message = "First error",
                Exception = "Exception 1"
            };
            var error2 = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow.AddSeconds(1),
                Level = "Error",
                Category = "ErrorTest",
                Message = "Second error",
                Exception = "Exception 2"
            };

            // Act
            await _repository.AddAsync(error1);
            await _repository.AddAsync(error2);
            await _context.SaveChangesAsync();

            // Assert
            var allLogs = await _repository.GetAllAsync();
            allLogs.Should().HaveCount(2);
            allLogs.Should().Contain(l => l.Message == "First error");
            allLogs.Should().Contain(l => l.Message == "Second error");
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldRemoveLogEntry()
        {
            // Arrange
            var logEntry = new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "DeleteTest",
                Message = "To be deleted"
            };
            await _repository.AddAsync(logEntry);
            await _context.SaveChangesAsync();
            var logId = logEntry.Id;

            // Act
            await _repository.DeleteAsync(logEntry);
            await _context.SaveChangesAsync();

            // Assert
            var retrieved = await _repository.GetByIdAsync(logId);
            retrieved.Should().BeNull();
        }

        [TestMethod]
        public async Task GetByLevelAsync_ShouldReturnOnlyLogsWithSpecificLevel()
        {
            // Arrange
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Category = "Test",
                Message = "Error 1"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "Test",
                Message = "Info 1"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Category = "Test",
                Message = "Error 2"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByLevelAsync("Error");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(l => l.Level == "Error");
        }

        [TestMethod]
        public async Task GetByCategoryAsync_ShouldReturnOnlyLogsWithSpecificCategory()
        {
            // Arrange
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "PersonService",
                Message = "Message 1"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "AstronautService",
                Message = "Message 2"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Category = "PersonService",
                Message = "Message 3"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCategoryAsync("PersonService");

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(l => l.Category == "PersonService");
        }

        [TestMethod]
        public async Task GetByDateRangeAsync_ShouldReturnLogsInTimeRange()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-2);
            var endDate = DateTime.UtcNow.AddDays(-1);

            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow.AddDays(-3),
                Level = "Information",
                Category = "Test",
                Message = "Too old"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow.AddDays(-1.5),
                Level = "Information",
                Category = "Test",
                Message = "In range"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "Test",
                Message = "Too recent"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByDateRangeAsync(startDate, endDate);

            // Assert
            result.Should().HaveCount(1);
            result.First().Message.Should().Be("In range");
        }

        [TestMethod]
        public async Task GetByCorrelationIdAsync_ShouldReturnLogsWithSameCorrelationId()
        {
            // Arrange
            var correlationId = Guid.NewGuid().ToString();

            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "Test",
                Message = "Message 1",
                CorrelationId = correlationId
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "Test",
                Message = "Message 2",
                CorrelationId = Guid.NewGuid().ToString()
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Error",
                Category = "Test",
                Message = "Message 3",
                CorrelationId = correlationId
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCorrelationIdAsync(correlationId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(l => l.CorrelationId == correlationId);
        }

        [TestMethod]
        public async Task GetRecentAsync_ShouldReturnMostRecentLogs()
        {
            // Arrange
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow.AddMinutes(-10),
                Level = "Information",
                Category = "Test",
                Message = "Oldest"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow.AddMinutes(-5),
                Level = "Information",
                Category = "Test",
                Message = "Middle"
            });
            await _repository.AddAsync(new LogEntryEntity
            {
                Timestamp = DateTime.UtcNow,
                Level = "Information",
                Category = "Test",
                Message = "Newest"
            });
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetRecentAsync(2);

            // Assert
            result.Should().HaveCount(2);
            result.First().Message.Should().Be("Newest");
            result.Last().Message.Should().Be("Middle");
        }
    }
}
