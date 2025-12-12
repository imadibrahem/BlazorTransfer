using BlazorTransfer.Shared;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorTransfer.Client.Services;

public class TransferService
{
    private readonly HttpClient _http;

    public TransferService(HttpClient http)
    {
        _http = http;
    }

    public async Task<FileUploadResult?> UploadAsync(IReadOnlyList<IBrowserFile> files)
    {
        var content = new MultipartFormDataContent();

        foreach (var file in files)
        {
            var stream = file.OpenReadStream(long.MaxValue);
            var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(streamContent, "files", file.Name);
        }

        var response = await _http.PostAsync("api/transfer/upload", content);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<FileUploadResult>();
    }
}