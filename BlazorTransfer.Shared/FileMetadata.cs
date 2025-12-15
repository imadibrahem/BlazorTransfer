namespace BlazorTransfer.Shared
{

    public class FileMetadata
    {
       
        public string FileName { get; set; } = default!;
        public long Size { get; set; }
        public string ContentType { get; set; } = default!;
        public string DownloadUrl { get; set; } = default!;
    }
}