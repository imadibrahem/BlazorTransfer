namespace BlazorTransfer.Api.Services
{
    public class FileCleanupWorker : BackgroundService
    {
        private readonly FileStorageService _fileStorageService;
        private readonly ILogger<FileCleanupWorker> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromHours(1);
        private readonly TimeSpan _fileMaxAge = TimeSpan.FromHours(24);

        public FileCleanupWorker(FileStorageService fileStorageService, ILogger<FileCleanupWorker> logger)
        {
            _fileStorageService = fileStorageService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("File cleanup worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting file cleanup.");
                    _fileStorageService.DeleteOlderThan(_fileMaxAge);
                    _logger.LogInformation("File cleanup completed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during file cleanup.");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("File cleanup worker stopping.");
        }
    }
}