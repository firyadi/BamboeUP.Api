using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Presentation.Modules.Controllers
{
    [Route("api/people")]
    [ApiController]
    public partial class PeopleController(IServiceModulesManager service) : ControllerBase
    {

        [HttpGet]
        [SwaggerOperation(Summary = "Get All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.PersonService.GetAllPeopleAsync(trackChanges: false);
            return Ok(data);
        }

        [HttpGet("{guid:guid}")]
        [SwaggerOperation(Summary = "Get By Guid")]
        public async Task<IActionResult> GetByGuid(Guid guid)
        {
            var data = await service.PersonService.GetPersonByGuidAsync(guid, trackChanges: false);
            if (data is null) return NotFound();
            return Ok(data);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create")]
        public async Task<IActionResult> Create([FromBody] PersonForCreationDto input)
        {
            var created = await service.PersonService.CreatePersonAsync(input);
            return CreatedAtAction(nameof(GetByGuid), new { guid = created.PersonGuid }, created);
        }

        [HttpPut("{guid:guid}")]
        [SwaggerOperation(Summary = "Update")]
        public async Task<IActionResult> Update(Guid guid, [FromBody] PersonForUpdateDto input)
        {
            await service.PersonService.UpdatePersonAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{guid:guid}")]
        [SwaggerOperation(Summary = "Soft Delete")]
        public async Task<IActionResult> SoftDelete(Guid guid, [FromBody] PersonForDeleteDto input)
        {
            await service.PersonService.DeletePersonAsync(guid, input, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("admin/{guid:guid}")]
        [SwaggerOperation(Summary = "Hard Delete (Admin)")]
        public async Task<IActionResult> HardDelete(Guid guid)
        {
            await service.PersonService.DeletePersonByAdminAsync(guid, trackChanges: false);
            return NoContent();
        }

        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search")]
        public async Task<IActionResult> Search([FromQuery] PersonSearchDto input)
        {
            var result = await service.PersonService.SearchPersonAsync(
                input.FirstName, input.FirstNameSearchType.ToString(),
                input.MiddleName, input.MiddleNameSearchType.ToString(),
                input.LastName, input.LastNameSearchType.ToString(),
                input.PreTitle, input.PreTitleSearchType.ToString(),
                input.PostTitle, input.PostTitleSearchType.ToString(),
                input.PersonName, input.PersonNameSearchType.ToString(),
                input.BirthName, input.BirthNameSearchType.ToString(),
                input.PlaceofBirth, input.PlaceofBirthSearchType.ToString(),
                input.BirthDate, input.BirthDateSearchType.ToString(),
                input.NationalIdNo, input.NationalIdNoSearchType.ToString(),
                input.SrGender, input.SrGenderSearchType.ToString(),
                input.SrReligion, input.SrReligionSearchType.ToString(),
                input.SrSalutation, input.SrSalutationSearchType.ToString(),
                input.SrBloodType, input.SrBloodTypeSearchType.ToString(),
                input.SrMaritalStatus, input.SrMaritalStatusSearchType.ToString()
                // ── FK Search pass-through ──
            );
            return Ok(result);
        }

        [HttpPost("{personGuid:guid}/photo")]
        [SwaggerOperation(Summary = "Upload person photo via FileStorage")]
        [RequestSizeLimit(20_000_000)]
        public async Task<IActionResult> UploadPhoto(Guid personGuid, IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
                return BadRequest(FileOperationResult.Fail("File is required.", "VALIDATION", ["File is empty."]));

            await using var stream = file.OpenReadStream();
            var request = new FileUploadRequest
            {
                Content = stream,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                Length = file.Length
            };

            var result = await service.PersonService.UploadPhotoAsync(personGuid, request, cancellationToken);
            if (!result.Success)
            {
                if (string.Equals(result.ErrorCode, "NOT_FOUND", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete("{personGuid:guid}/photo")]
        [SwaggerOperation(Summary = "Remove person photo")]
        public async Task<IActionResult> RemovePhoto(Guid personGuid, CancellationToken cancellationToken)
        {
            var result = await service.PersonService.RemovePhotoAsync(personGuid, deletePhysicalFile: true, cancellationToken);
            if (!result.Success)
            {
                if (string.Equals(result.ErrorCode, "NOT_FOUND", StringComparison.OrdinalIgnoreCase))
                    return NotFound(result);
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
