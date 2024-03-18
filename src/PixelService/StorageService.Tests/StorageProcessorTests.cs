using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using StorageService.Services;

namespace StorageService.Tests
{
	public class StorageProcessorTests
	{
		private Mock<IStorageLoggerService> _mockStorageLoggerService;
		private Mock<IConnection> _mockRabbitConnection;
		private Mock<IModel> _mockChannel;
		private StorageProcessor _sut;


		[SetUp]
		public void Setup()
		{
			_mockStorageLoggerService = new Mock<IStorageLoggerService>();
			_sut = new StorageProcessor(_mockStorageLoggerService.Object);


			_mockRabbitConnection = new Mock<IConnection>();

			_mockChannel = new Mock<IModel>();
			_mockRabbitConnection.Setup(x => x.CreateModel()).Returns(_mockChannel.Object);
		}

		[Test]
		public void StartsMonitoring_Should_ConnectToRabbit()
		{
			// Arrange

			// Act
			_sut.StartsMonitoring(_mockRabbitConnection.Object);

			// Assert
			_mockChannel.Verify(x => x.QueueDeclare("visit_queue", true, false, false, null), Times.Once);
		}
	}
}
