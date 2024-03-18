using RabbitMQ.Client;
using StorageService.Services;

namespace StorageService
{
    class Program
	{
		static void Main(string[] args)
		{
			var config = new ConfigurationBuilder()
				 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				 .Build();

			var factory = new ConnectionFactory() 
			{ 
				HostName = config.GetValue<string>("RabbitMQ:HostName") ,
				UserName = config.GetValue<string>("RabbitMQ:Username"),
				Password = config.GetValue<string>("RabbitMQ:Password"),
			};

			using var connection = factory.CreateConnection();

			var processor = new StorageProcessor(StorageLoggerService.Instance);

			processor.StartsMonitoring(connection);

			Console.WriteLine("StorageProcessor is listening for messages. Press any key to exit.");
			Console.ReadKey();
		}
	}
}
