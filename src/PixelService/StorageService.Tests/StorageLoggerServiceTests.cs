using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using StorageService.Services;

namespace StorageService.Tests
{
	public class StorageLoggerServiceTests
	{
		private Mock<ILogger> _mockLogger;
		private StorageLoggerService _sut;

		[SetUp]
		public void Setup()
		{
			_mockLogger = new Mock<ILogger>();
			_sut = StorageLoggerService.Instance;

			_sut.GetType()
				.GetField("_logWriter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
				.SetValue(_sut, new Lazy<ILogger>(() => _mockLogger.Object));
		}

		[Test]
		public async Task LogAsync_Should_WriteToILogger()
		{
			// Arrange
			var message = "Test Referrer|Test User Agent|127.0.0.1";

			// Act
			await _sut.LogAsync(message);

			// Assert
			_mockLogger.Verify(lw => lw.WriteAsync(message), Times.Once);
		}
	}
}
