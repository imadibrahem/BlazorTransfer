
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlazorTransfer.Shared;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorTransfer.Client.Services
{
    public class TransferService
    {
        private readonly HttpClient _http;

        public TransferService(HttpClient http)
        {
            _http = http;
        }

         public async Task<FileUploadResult> UploadFilesAsync(IBrowserFile[] files)
        {
            using var content = new MultipartFormDataContent();
           
            foreach (var f in files)
            {
             var ms = new MemoryStream();
             await f.OpenReadStream(f.Size).CopyToAsync(ms);
             ms.Position = 0;

             var streamContent = new StreamContent(ms);
             streamContent.Headers.ContentType =
             new MediaTypeHeaderValue(f.ContentType);

             content.Add(streamContent, "files", f.Name);
           }
              
           var response = await _http.PostAsync("api/transfer/upload", content);
           response.EnsureSuccessStatusCode();

           return await response.Content.ReadFromJsonAsync<FileUploadResult>()
           ?? throw new Exception("Invalid response");
           
        }

        public async Task<Stream> DownloadAsync(string id)
        {
            return await _http.GetStreamAsync($"api/transfer/download/{id}");
        }
    }
}