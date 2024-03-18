namespace StorageService.Services
{
	public interface IStorageLoggerService
    {
        Task LogAsync(string visitData);
    }
}
