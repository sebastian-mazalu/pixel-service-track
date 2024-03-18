using Microsoft.OpenApi.Models;
using RabbitMQ.Client;

namespace PixelService
{
	public class Program
	{
		private static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "PixelService", Version = "v1" });
			});

			// Add RabbitMQ client
			builder.Services.AddSingleton(sp =>
			{
				var configuration = sp.GetRequiredService<IConfiguration>();
				var rabbitConfig = configuration.GetSection("RabbitMQ");
				var factory = new ConnectionFactory
				{
					HostName = rabbitConfig["HostName"],
					UserName = rabbitConfig["UserName"],
					Password = rabbitConfig["Password"]
				};

				return factory.CreateConnection();
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/error");
			}

			app.UseSwagger();
			app.UseSwaggerUI(c =>
			{
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "PixelService v1");
				c.RoutePrefix = string.Empty;
			});

			app.UseRouting();

			PixelServiceEndpoints.ConfigureEndpoints(app);

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.Run();
		}
	}
}