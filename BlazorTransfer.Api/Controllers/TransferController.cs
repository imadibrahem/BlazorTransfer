using BlazorTransfer.Api.Services;
using BlazorTransfer.Shared;
using Microsoft.AspNetCore.Mvc;


namespace BlazorTransfer.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferController : ControllerBase
    {
        private readonly FileStorageService _storage;

        public TransferController(FileStorageService storage)
        {
             _storage = storage;
        } 

        [HttpPost("upload")]
        public async Task<ActionResult<FileUploadResult>> Upload()
        {

        foreach (var file in Request.Form.Files)
        {
            if (file.Length > 2L * 1024 * 1024 * 1024)
            {
               return BadRequest($"File '{file.FileName}' exceeds 2 GB limit.");
            }
        }

        var files = Request.Form.Files;
        if (files == null || files.Count == 0)
            return BadRequest("No files");

        var result = await _storage.SaveAsync(files);

        var downloadUrl = $"{Request.Scheme}://{Request.Host}/api/transfer/download/{result.TransferId}";

        var response = new FileUploadResult
        {
            TransferId = result.TransferId,
            DownloadUrl = downloadUrl,
            Files = result.Files
        };

        return Ok(response);
      }

        [HttpGet("download/{id}")]
        public IActionResult Download(string id)
        {
            var zip = _storage.GetZipStream(id);
            if (zip == null) return NotFound();
            return File(zip.Value.Stream, zip.Value.ContentType, zip.Value.FileName);
        }
    }
}

