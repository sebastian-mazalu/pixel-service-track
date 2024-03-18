namespace StorageService.Services
{
    public class FileLogger : ILogger
    {
        private readonly object _lockObject = new object();
        private readonly string _logFilePath;

        public FileLogger(string logFilePath)
        {
            _logFilePath = logFilePath;
        }

        public Task WriteAsync(string message)
        {
            try
            {
                var logEntry = $"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ} | {message}\n";
                lock (_lockObject)
                {
                    return File.AppendAllTextAsync(_logFilePath, logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while logging visit: {ex.Message}");
                return Task.CompletedTask;
            }
        }
    }
}
