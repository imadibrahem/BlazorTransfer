using BlazorTransfer.Shared;

namespace BlazorTransfer.Api.Services
{
    public class FileStorageService
    {
        private readonly string _basePath;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FileStorageService> _logger;

        public FileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ILogger<FileStorageService> logger)
        {
            _basePath = Path.Combine(env.ContentRootPath, "Storage");
            Directory.CreateDirectory(_basePath);
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<FileUploadResult> SaveAsync(IFormFileCollection files)
        {
            string transferId = Guid.NewGuid().ToString("N");

            string folder = Path.Combine(_basePath, transferId);
            Directory.CreateDirectory(folder);

            List<FileMetadata> metadata = new();

            foreach (var file in files)
            {
                string path = Path.Combine(folder, file.FileName);
                using var stream = new FileStream(path, FileMode.Create);
                await file.CopyToAsync(stream);

                metadata.Add(new FileMetadata
                {
                    FileName = file.FileName,
                    Size = file.Length,
                    ContentType = file.ContentType
            });
            }

            return new FileUploadResult
            {
                TransferId = transferId,
                Files = metadata
            };

        }

        public (Stream Stream, string FileName, string ContentType)? GetFileStream(string id)
        {
            var dir = Path.Combine(_basePath, id);
            if (!Directory.Exists(dir)) return null;
            var file = Directory.GetFiles(dir).FirstOrDefault();
            if (file == null) return null;
            var stream = File.OpenRead(file);
            return (stream, Path.GetFileName(file), "application/octet-stream");
        }

        public void DeleteOlderThan(TimeSpan maxAge)
        {
            foreach (var dir in Directory.GetDirectories(_basePath))
            {
                try
                {
                    var created = Directory.GetCreationTimeUtc(dir);
                    if (DateTime.UtcNow - created > maxAge)
                        Directory.Delete(dir, true);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed cleanup for {dir}", dir);
                }
            }
        }
    }
}