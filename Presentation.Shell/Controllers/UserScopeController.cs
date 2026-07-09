using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Shell;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Shell.Controllers;

[Authorize]
[Route("api/user-scope")]
[ApiController]
public class UserScopeController : ControllerBase
{
    private readonly IServiceShellManager _service;

    public UserScopeController(IServiceShellManager service)
    {
        _service = service;
    }

    [HttpGet("companies")]
    [SwaggerOperation(Summary = "Get accessible companies", Description = "Returns companies the current user may access.")]
    public async Task<IActionResult> GetAccessibleCompanies()
    {
        var data = await _service.UserScopeService.GetAccessibleCompaniesAsync();
        return Ok(data);
    }

    [HttpGet("companies/{companyGuid:guid}/offices")]
    [SwaggerOperation(Summary = "Get accessible offices", Description = "Returns offices the current user may access within the given company.")]
    public async Task<IActionResult> GetAccessibleOffices(Guid companyGuid)
    {
        var data = await _service.UserScopeService.GetAccessibleOfficesByCompanyGuidAsync(companyGuid);
        return Ok(data);
    }

    [HttpGet("organization-units")]
    [SwaggerOperation(Summary = "Get accessible organization units", Description = "Returns organization units visible to the current user.")]
    public async Task<IActionResult> GetAccessibleOrganizationUnits()
    {
        var data = await _service.UserScopeService.GetAccessibleOrganizationUnitsAsync();
        return Ok(data);
    }
}
