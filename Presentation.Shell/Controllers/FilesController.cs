using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
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
    }
}
