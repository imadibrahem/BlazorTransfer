using System.Net;
using System.Net.Http.Headers;

public class ProgressableStreamContent : HttpContent
{
    private const int DefaultBufferSize = 81920;
    private readonly Stream _stream;
    private readonly Action<long> _progress;

    public ProgressableStreamContent(Stream stream, Action<long> progress)
    {
        _stream = stream;
        _progress = progress;
    }

    protected override async Task SerializeToStreamAsync(Stream target, TransportContext? context)
    {
        var buffer = new byte[DefaultBufferSize];
        long uploaded = 0;

        int read;
        while ((read = await _stream.ReadAsync(buffer)) > 0)
        {
            await target.WriteAsync(buffer.AsMemory(0, read));
            uploaded += read;
            _progress(uploaded);
        }
    }

    protected override bool TryComputeLength(out long length)
    {
        length = _stream.Length;
        return true;
    }
}
