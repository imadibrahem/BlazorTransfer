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
        public async Task<ActionResult<FileUploadResult>> Upload(
            [FromQuery] string? transferId)
        {
            var files = Request.Form.Files;
            if (files.Count == 0)
                return BadRequest("No files");
            

             
            var result = new FileUploadResult();
            try
            {
               result = await _storage.SaveAsync(files, transferId);
 
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing files: {ex.Message}");
            }
            
            var downloadUrl =
                $"{Request.Scheme}://{Request.Host}/api/transfer/download/{result.TransferId}";

            return Ok(new FileUploadResult
            {
                TransferId = result.TransferId,
                DownloadUrl = downloadUrl,
                Files = result.Files
            });
        }

        [HttpGet("download/{id}")]
        public IActionResult Download(string id)
        {
            var zip = _storage.GetZipStream(id);
            if (zip == null) return NotFound();
            return File(zip.Value.Stream, zip.Value.ContentType, zip.Value.FileName);
        }

        [HttpGet("info/{id}")]
        public ActionResult<FileUploadResult> GetInfo(string id)
        {
            var info = _storage.GetTransferInfo(id);
            if (info == null)
                return NotFound();

            return Ok(info);
        }

    }
}

