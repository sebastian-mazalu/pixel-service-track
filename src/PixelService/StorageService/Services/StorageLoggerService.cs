namespace StorageService.Services
{
    public class StorageLoggerService : IStorageLoggerService
    {
        private readonly string _logFilePath;
        private readonly Lazy<ILogger> _logWriter;

        private StorageLoggerService(string logFilePath, ILogger logWriter)
        {
            _logFilePath = logFilePath;
            _logWriter = new Lazy<ILogger>(() => logWriter);
        }

        private static readonly Lazy<StorageLoggerService> _instance = new(() =>
        {
            var config = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                 .Build();

            var logFilePath = config.GetValue<string>("LogFilePath") ?? "/tmp/visits.log";

            return new StorageLoggerService(logFilePath, new FileLogger(logFilePath));
        });

        public static StorageLoggerService Instance => _instance.Value;

        public Task LogAsync(string message)
        {
            return _logWriter.Value.WriteAsync(message);
        }
    }
}
