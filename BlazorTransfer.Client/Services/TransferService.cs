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

    public async Task<FileUploadResult?> UploadAsync(
        IReadOnlyList<IBrowserFile> files,
        Action<string, long, long> onProgress)
    {
        var content = new MultipartFormDataContent();

        foreach (var file in files)
        {
            var stream = file.OpenReadStream(MaxFileSize);

            var progressContent = new ProgressableStreamContent(
                stream,
                uploaded => onProgress(file.Name, uploaded, file.Size)
            );

            progressContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(progressContent, "files", file.Name);
        }

        var response = await _http.PostAsync("api/transfer/upload", content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<FileUploadResult>();
    }

}