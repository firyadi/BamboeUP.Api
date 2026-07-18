using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace BamboeUp.Api.Controllers
{
    [Route("api/fileStorages")]
    [ApiController]
    public partial class FileStoragesController(IServiceShellManager service) : ControllerBase
    {
        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.FileStorageService.GetAllFileStoragesAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.FileStorageService.GetFileStorageByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] FileStorageForCreationDto input)
        {
            var created = await service.FileStorageService.CreateFileStorageAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.FileStorageGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] FileStorageForUpdateDto input)
        {
            await service.FileStorageService.UpdateFileStorageAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] FileStorageForDeleteDto input)
        {
            await service.FileStorageService.DeleteFileStorageAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.FileStorageService.DeleteFileStorageByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] FileStorageSearchDto input)
        {
            var result = await service.FileStorageService.SearchFileStorageAsync(
                input.OriginalFileName, input.OriginalFileNameSearchType.ToString(),
                input.StoredFileName, input.StoredFileNameSearchType.ToString(),
                input.Extension, input.ExtensionSearchType.ToString(),
                input.MimeType, input.MimeTypeSearchType.ToString(),
                input.FileSize, input.FileSizeSearchType.ToString(),
                input.StorageProvider, input.StorageProviderSearchType.ToString(),
                input.RelativePath, input.RelativePathSearchType.ToString(),
                input.Width, input.WidthSearchType.ToString(),
                input.Height, input.HeightSearchType.ToString(),
                input.IsImage, input.IsImageSearchType.ToString(),
                input.FileHash, input.FileHashSearchType.ToString(),
                input.DownloadCount, input.DownloadCountSearchType.ToString(),
                input.LastAccessTime, input.LastAccessTimeSearchType.ToString(),
                input.IsTemporary, input.IsTemporarySearchType.ToString(),
                input.Description, input.DescriptionSearchType.ToString()
, input.FileCategoryName, input.FileCategoryNameSearchType.ToString()

            );
            return Ok(result);
        }
    }
}
