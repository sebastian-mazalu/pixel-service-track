using System;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;

namespace PixelService.Tests
{
	public class ProgramTests
	{
		private Mock<IConnection> _mockConnection;
		private Mock<IModel> _mockModel;
		private Mock<IConnectionFactory> _factoryMock;
		private Mock<IServiceProvider> _mockServiceProvider;

		private WebApplicationFactory<Program> _factory;

		[SetUp]
		public void Setup()
		{

			_mockConnection = new Mock<IConnection>();
			_mockModel = new Mock<IModel>();
			_mockConnection.Setup(x => x.CreateModel()).Returns(_mockModel.Object);
			_factoryMock = new Mock<IConnectionFactory>();
			_factoryMock.Setup(x => x.CreateConnection()).Returns(_mockConnection.Object);

			_mockServiceProvider = new Mock<IServiceProvider>();
			_mockServiceProvider.Setup(x => x.GetService(typeof(IConnectionFactory))).Returns(_factoryMock.Object);

			_factory = new WebApplicationFactory<Program>();
		}

		[TearDown]
		public void TearDown()
		{
			_factory.Dispose();
		}

		[Test]
		public async Task TrackEndpoint_Returns_OnePixelGifImage()
		{
			// Arrange
			var app = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					services.AddSingleton(_mockConnection.Object);
				});
			}).CreateClient();

			app.DefaultRequestHeaders.Add("Referer", "https://example.com");
			app.DefaultRequestHeaders.Add("User-Agent", "Test User Agent");

			// Act
			var response = await app.GetAsync("/track");
			var reponseImageContentString = await response.Content.ReadAsStringAsync();

			// Assert
			response.StatusCode.Should().Be(HttpStatusCode.OK);
			response.Content.Headers.ContentType.MediaType.Should().Be("image/gif");
			reponseImageContentString.Should().Be(PixelServiceEndpoints.ONE_PIXEL_GIF_IMAGE);
		}

		[Test]
		public async Task TrackEndpoint_SendsDataToRabbitMQ()
		{
			// Arrange
			var app = _factory.WithWebHostBuilder(builder =>
			{
				builder.ConfigureServices(services =>
				{
					services.AddSingleton(_mockConnection.Object);
				});
			}).CreateClient();

			app.DefaultRequestHeaders.Add("Referer", "https://example.com");
			app.DefaultRequestHeaders.Add("User-Agent", "Test User Agent");

			// Act
			var response = await app.GetAsync("/track");

			// Assert
			_mockConnection.Verify(x => x.CreateModel(), Times.Once);

			_mockModel.Verify(x => x.QueueDeclare("visit_queue", true, false, false, null), Times.Once);
			_mockModel.Verify(x => x.QueueDeclare("visit_queue", true, false, false, null), Times.Once);
			_mockConnection.Verify(x => x.Dispose(), Times.Once);
		}
	}
}