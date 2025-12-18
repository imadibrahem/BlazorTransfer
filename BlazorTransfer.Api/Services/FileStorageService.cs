using System.IO.Compression;
using BlazorTransfer.Shared;

namespace BlazorTransfer.Api.Services
{
    public class FileStorageService
    {
        private readonly string _basePath;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FileStorageService> _logger;
        private const long MaxTotalUploadSize = 5L * 1024 * 1024 * 1024; // 5 GB

        public FileStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ILogger<FileStorageService> logger)
        {
            _basePath = Path.Combine(env.ContentRootPath, "Storage");
            Directory.CreateDirectory(_basePath);
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<FileUploadResult> SaveAsync(
            IFormFileCollection files,
            string? transferId)
        {
            transferId ??= Guid.NewGuid().ToString("N");

            var folder = Path.Combine(_basePath, transferId);
            Directory.CreateDirectory(folder);

            var existingSize = Directory.EnumerateFiles(folder)
                .Select(f => new FileInfo(f).Length)
                .Sum();


            long incomingSize = files.Sum(f => f.Length);
            if (existingSize + incomingSize > MaxTotalUploadSize)
            {
                throw new InvalidOperationException("Total upload size exceeds the limit.");
            }

            var metadata = new List<FileMetadata>();

            foreach (var file in files)
            {
                var path = Path.Combine(folder, file.FileName);
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
        
        public (Stream Stream, string FileName, string ContentType)? GetZipStream(string id)
        {
            var dir = Path.Combine(_basePath, id);
            if (!Directory.Exists(dir)) return null;

            var zipStream = new MemoryStream();

            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var filePath in Directory.GetFiles(dir))
                {
                    var entry = archive.CreateEntry(Path.GetFileName(filePath), CompressionLevel.Fastest);
                    using var entryStream = entry.Open();
                    using var fileStream = File.OpenRead(filePath);
                    fileStream.CopyTo(entryStream);
                }
            }

            zipStream.Position = 0;

            return (zipStream, $"transfer-{id}.zip", "application/zip");
        }

        public FileUploadResult? GetTransferInfo(string id)
        {
            var dir = Path.Combine(_basePath, id);
            if (!Directory.Exists(dir))
                return null;

            var files = Directory.GetFiles(dir)
                .Select(f => new FileMetadata
                {
                    FileName = Path.GetFileName(f),
                    Size = new FileInfo(f).Length,
                    ContentType = "application/octet-stream"
                })
                .ToList();
            return new FileUploadResult
            {
                TransferId = id,
                Files = files
            };
        }
    }
}