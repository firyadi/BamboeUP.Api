using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Shared.DataTransferObjects;

namespace Presentation.Shell.Controllers;

[Route("api/shared-parameter-catalog")]
[ApiController]
public class SharedParameterCatalogController : ControllerBase
{
    private readonly ISharedParameterCatalogService _service;

    public SharedParameterCatalogController(ISharedParameterCatalogService service)
    {
        _service = service;
    }

    [HttpGet("snapshot")]
    public async Task<IActionResult> GetSnapshot()
        => Ok(await _service.GetSnapshotAsync());

    [HttpGet]
    public async Task<IActionResult> GetLibrary()
        => Ok(await _service.GetLibraryAsync());

    [HttpGet("profiles")]
    public async Task<IActionResult> GetProfiles()
        => Ok(await _service.GetProfilesAsync());

    [HttpGet("profiles/{token}/fields")]
    public async Task<IActionResult> ApplyProfile(string token, [FromQuery] string? audience = "Process")
        => Ok(await _service.ApplyProfileAsync(token, audience));

    [HttpGet("memberships")]
    public async Task<IActionResult> GetMemberships([FromQuery] string? profileToken = null)
        => Ok(await _service.GetMembershipsAsync(profileToken));

    [HttpPost]
    public async Task<IActionResult> CreateField([FromBody] SharedParameterCatalogFieldForUpsertDto input)
    {
        var row = await _service.CreateFieldAsync(input);
        return CreatedAtAction(nameof(GetLibrary), new { guid = row.SharedParameterCatalogGuid }, row);
    }

    [HttpPut("{guid:guid}")]
    public async Task<IActionResult> UpdateField(Guid guid, [FromBody] SharedParameterCatalogFieldForUpsertDto input)
    {
        await _service.UpdateFieldAsync(guid, input);
        return NoContent();
    }

    [HttpDelete("{guid:guid}")]
    public async Task<IActionResult> DeleteField(Guid guid, [FromQuery] long deletedById = 1)
    {
        await _service.DeleteFieldAsync(guid, deletedById);
        return NoContent();
    }

    [HttpPost("profiles")]
    public async Task<IActionResult> CreateProfile([FromBody] SharedParameterProfileForUpsertDto input)
    {
        var row = await _service.CreateProfileAsync(input);
        return Ok(row);
    }

    [HttpPut("profiles/{guid:guid}")]
    public async Task<IActionResult> UpdateProfile(Guid guid, [FromBody] SharedParameterProfileForUpsertDto input)
    {
        await _service.UpdateProfileAsync(guid, input);
        return NoContent();
    }

    [HttpDelete("profiles/{guid:guid}")]
    public async Task<IActionResult> DeleteProfile(Guid guid, [FromQuery] long deletedById = 1)
    {
        await _service.DeleteProfileAsync(guid, deletedById);
        return NoContent();
    }

    [HttpPost("memberships")]
    public async Task<IActionResult> CreateMembership([FromBody] SharedParameterProfileFieldForUpsertDto input)
    {
        var row = await _service.CreateMembershipAsync(input);
        return Ok(row);
    }

    [HttpPut("memberships/{guid:guid}")]
    public async Task<IActionResult> UpdateMembership(Guid guid, [FromBody] SharedParameterProfileFieldForUpsertDto input)
    {
        await _service.UpdateMembershipAsync(guid, input);
        return NoContent();
    }

    [HttpDelete("memberships/{guid:guid}")]
    public async Task<IActionResult> DeleteMembership(Guid guid, [FromQuery] long deletedById = 1)
    {
        await _service.DeleteMembershipAsync(guid, deletedById);
        return NoContent();
    }
}
