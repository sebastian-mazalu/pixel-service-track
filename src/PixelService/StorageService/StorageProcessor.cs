using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StorageService.Services;

namespace StorageService
{
	public class StorageProcessor
	{
		private readonly IStorageLoggerService _storageService;

		public StorageProcessor(IStorageLoggerService storageService)
		{
			_storageService = storageService;
		}

		public void StartsMonitoring(IConnection rabbitConnection)
		{
			using var channel = rabbitConnection.CreateModel();

			channel.QueueDeclare("visit_queue", durable: true, false, false, null);
			var consumer = new EventingBasicConsumer(channel);

			consumer.Received += async (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);
				await _storageService.LogAsync(message);
			};

			channel.BasicConsume("visit_queue", true, consumer);
		}
	}
}
