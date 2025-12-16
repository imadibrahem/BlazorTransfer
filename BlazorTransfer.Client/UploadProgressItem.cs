public class UploadProgressItem
{
    public string FileName { get; set; } = "";
    public long TotalBytes { get; set; }
    public long UploadedBytes { get; set; }

    public int Percentage =>
        TotalBytes == 0 ? 0 : (int)(UploadedBytes * 100 / TotalBytes);
}