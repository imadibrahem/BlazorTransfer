namespace BlazorTransfer.Shared
{
    public class FileUploadResult
    {
        public string TransferId { get; set; } = default!;
        public string DownloadUrl { get; set; } = default!;
        public IEnumerable<FileMetadata> Files { get; set; } = Array.Empty<FileMetadata>();
    }

 
}