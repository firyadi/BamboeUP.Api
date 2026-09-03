using Microsoft.AspNetCore.Mvc;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;
using Swashbuckle.AspNetCore.Annotations;

namespace Presentation.Modules.Controllers;

[Route("api/processes")]
[ApiController]
public class ProcessesController(IServiceModulesManager service) : ControllerBase
{
    private IProcessExecutionService Process => service.ProcessExecutionService;

    [HttpGet]
    [SwaggerOperation(Summary = "List process definitions")]
    public async Task<IActionResult> GetDefinitions([FromQuery] string? processCode = null)
    {
        var data = await Process.GetDefinitionsAsync(processCode);
        return Ok(data);
    }

    [HttpGet("{guid:guid}")]
    [SwaggerOperation(Summary = "Get process definition with parameters and steps")]
    public async Task<IActionResult> GetDefinition(Guid guid)
    {
        var data = await Process.GetDefinitionAsync(guid);
        if (data is null) return NotFound();
        return Ok(data);
    }

    [HttpPost("{guid:guid}/validate")]
    [SwaggerOperation(Summary = "L2 business validation")]
    public async Task<IActionResult> Validate(Guid guid, [FromBody] ProcessValidateRequestDto input)
    {
        var result = await Process.ValidateAsync(guid, input);
        return Ok(result);
    }

    [HttpPost("{guid:guid}/execute")]
    [SwaggerOperation(Summary = "Create and queue/run process execution")]
    public async Task<IActionResult> Execute(Guid guid, [FromBody] ProcessExecuteRequestDto input)
    {
        Guid? idempotencyKey = null;
        if (Request.Headers.TryGetValue("Idempotency-Key", out var keyHeader)
            && Guid.TryParse(keyHeader.ToString(), out var parsed))
        {
            idempotencyKey = parsed;
        }

        var created = await Process.ExecuteAsync(guid, input, idempotencyKey);
        return CreatedAtAction(nameof(GetExecution), new { guid = created.ProcessExecutionGuid }, created);
    }

    [HttpGet("executions")]
    [SwaggerOperation(Summary = "Monitor process executions")]
    public async Task<IActionResult> SearchExecutions([FromQuery] ProcessExecutionSearchDto input)
    {
        var data = await Process.SearchExecutionsAsync(input);
        return Ok(data);
    }

    [HttpGet("executions/{guid:guid}")]
    [SwaggerOperation(Summary = "Get execution detail")]
    public async Task<IActionResult> GetExecution(Guid guid)
    {
        var data = await Process.GetExecutionAsync(guid);
        if (data is null) return NotFound();
        return Ok(data);
    }

    [HttpGet("executions/{guid:guid}/messages")]
    [SwaggerOperation(Summary = "Get execution messages")]
    public async Task<IActionResult> GetMessages(Guid guid)
    {
        var data = await Process.GetMessagesAsync(guid);
        return Ok(data);
    }

    [HttpGet("executions/{guid:guid}/result-details")]
    [SwaggerOperation(Summary = "Get execution result detail rows")]
    public async Task<IActionResult> GetResultDetails(Guid guid)
    {
        var data = await Process.GetResultDetailsAsync(guid);
        return Ok(data);
    }

    [HttpPost("executions/{guid:guid}/cancel")]
    [SwaggerOperation(Summary = "Request cancellation")]
    public async Task<IActionResult> Cancel(Guid guid)
    {
        try
        {
            await Process.CancelAsync(guid);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { errorCode = "INVALID_TRANSITION", message = ex.Message });
        }
    }

    [HttpPost("executions/{guid:guid}/retry")]
    [SwaggerOperation(Summary = "Retry process execution")]
    public async Task<IActionResult> Retry(Guid guid, [FromBody] ProcessRetryRequestDto input)
    {
        try
        {
            var created = await Process.RetryAsync(guid, input);
            return CreatedAtAction(nameof(GetExecution), new { guid = created.ProcessExecutionGuid }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { errorCode = "RETRY_NOT_ALLOWED", message = ex.Message });
        }
    }
}
