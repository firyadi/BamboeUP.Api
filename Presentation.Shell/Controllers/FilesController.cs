using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController(IServiceShellManager service) : ControllerBase
    {
        [HttpGet("{fileStorageGuid:guid}")]
        [SwaggerOperation(Summary = "Download file by FileStorageGuid")]
        public async Task<IActionResult> Download(Guid fileStorageGuid, CancellationToken cancellationToken)
        {
            var result = await service.FileStorageService.OpenDownloadAsync(fileStorageGuid, cancellationToken);
            if (!result.Success || result.Data is null)
            {
                if (string.Equals(result.ErrorCode, "NOT_FOUND", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(result.ErrorCode, "FILE_MISSING", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result);

                return BadRequest(result);
            }

            return File(result.Data.Content, result.Data.ContentType, result.Data.DownloadFileName);
        }

        [HttpPost]
        [RequestSizeLimit(20 * 1024 * 1024)]
        [SwaggerOperation(Summary = "Upload a photo and return FileStorage metadata")]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                return BadRequest(FileOperationResult<FileStorageDto>.Fail("File is empty.", "EMPTY"));

            var category = await service.FileStorageService.ResolveFileCategoryIdAsync("Photo", cancellationToken) ?? 0;
            await using var stream = file.OpenReadStream();
            var uploaded = await service.FileStorageService.UploadAsync(
                new FileUploadRequest
                {
                    Content = stream,
                    OriginalFileName = string.IsNullOrWhiteSpace(file.FileName) ? "photo.jpg" : file.FileName,
                    ContentType = string.IsNullOrWhiteSpace(file.ContentType) ? "image/jpeg" : file.ContentType,
                    Length = file.Length
                },
                category,
                cancellationToken);

            if (!uploaded.Success)
                return BadRequest(uploaded);

            return Ok(uploaded);
        }
    }
}
