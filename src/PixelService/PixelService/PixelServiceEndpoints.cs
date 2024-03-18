using RabbitMQ.Client;
using System.Text;

namespace PixelService
{
	public class PixelServiceEndpoints
	{
		internal static string ONE_PIXEL_GIF_IMAGE = "data:image/gif;base64,R0lGODlhAQABAIAAAP///////yH5BAAAAAAALAAAAAABAAEAAAIBAAA=";

		public static void ConfigureEndpoints(WebApplication app)
		{
			app.MapGet("/track", (RequestDelegate)(async context =>
			{
				var request = context.Request;
				var response = context.Response;

				// Collect information
				var referrer = request.Headers["Referer"].ToString();
				var userAgent = request.Headers["User-Agent"].ToString();
				var ip = request?.HttpContext?.Connection?.RemoteIpAddress?.ToString();

				using var connection = context.RequestServices.GetRequiredService<IConnection>();
				using var channel = connection.CreateModel();
				channel.QueueDeclare("visit_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

				var message = Encoding.UTF8.GetBytes($"{referrer}|{userAgent}|{ip}");
				channel.BasicPublish(exchange: "", routingKey: "visit_queue", basicProperties: null, body: message);

				response.ContentType = "image/gif";
				await response.Body.WriteAsync(Encoding.ASCII.GetBytes((string)ONE_PIXEL_GIF_IMAGE));
			}));
		}
	}
}
