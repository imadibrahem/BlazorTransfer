using BlazorTransfer.Shared;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorTransfer.Client.Services;

public class TransferService
{
    private readonly HttpClient _http;
    private const long MaxFileSize = 2L * 1024 * 1024 * 1024;


    public TransferService(HttpClient http)
    {
        _http = http;
    }

    public async Task<FileUploadResult?> UploadSequentialAsync(
        IReadOnlyList<IBrowserFile> files,
        Action<string, long, long> onProgress)
    {
        string? transferId = null;
        FileUploadResult? lastResult = null;

        foreach (var file in files)
        {
            var content = new MultipartFormDataContent();

            using var stream = file.OpenReadStream(MaxFileSize);

            var progressContent = new ProgressableStreamContent(
                stream,
                uploaded => onProgress(file.Name, uploaded, file.Size));

            progressContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(progressContent, "files", file.Name);

            var url = "api/transfer/upload";
            if (transferId != null)
                url += $"?transferId={transferId}";

            var response = await _http.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            lastResult =
                await response.Content.ReadFromJsonAsync<FileUploadResult>();

            transferId ??= lastResult!.TransferId;
        }

        return lastResult;
    }


}