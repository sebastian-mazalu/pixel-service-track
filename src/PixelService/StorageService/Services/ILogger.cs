namespace StorageService.Services
{
    public interface ILogger
    {
        Task WriteAsync(string message);
    }
}
