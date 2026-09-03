using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Moq;
using Presentation.Modules.Controllers;
using Service.Contracts.Modules;
using Shared.DataTransferObjects;

namespace BamboeUp.Api.Tests;

public class ProcessesControllerTests
{
    [Fact]
    public async Task Execute_ReturnsCreatedAtAction_WithExecutionPayload()
    {
        var processService = new Mock<IProcessExecutionService>();
        var manager = new Mock<IServiceModulesManager>();
        manager.SetupGet(m => m.ProcessExecutionService).Returns(processService.Object);

        var definitionGuid = Guid.NewGuid();
        var expected = new ProcessExecutionDto
        {
            ProcessExecutionGuid = Guid.NewGuid(),
            ExecutionNo = "PR-2026-00000001",
            Status = "Queued"
        };

        processService
            .Setup(s => s.ExecuteAsync(definitionGuid, It.IsAny<ProcessExecuteRequestDto>(), null))
            .ReturnsAsync(expected);

        var sut = new ProcessesController(manager.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
        var response = await sut.Execute(definitionGuid, new ProcessExecuteRequestDto());

        var created = Assert.IsType<CreatedAtActionResult>(response);
        Assert.Equal(nameof(ProcessesController.GetExecution), created.ActionName);
        var dto = Assert.IsType<ProcessExecutionDto>(created.Value);
        Assert.Equal(expected.ProcessExecutionGuid, dto.ProcessExecutionGuid);
    }

    [Fact]
    public async Task Retry_WhenServiceThrowsInvalidOperation_ReturnsConflict()
    {
        var processService = new Mock<IProcessExecutionService>();
        var manager = new Mock<IServiceModulesManager>();
        manager.SetupGet(m => m.ProcessExecutionService).Returns(processService.Object);

        processService
            .Setup(s => s.RetryAsync(It.IsAny<Guid>(), It.IsAny<ProcessRetryRequestDto>()))
            .ThrowsAsync(new InvalidOperationException("Retry not allowed."));

        var sut = new ProcessesController(manager.Object);
        var response = await sut.Retry(Guid.NewGuid(), new ProcessRetryRequestDto());

        Assert.IsType<ConflictObjectResult>(response);
    }

    [Fact]
    public async Task Cancel_WhenServiceThrowsInvalidOperation_ReturnsConflict()
    {
        var processService = new Mock<IProcessExecutionService>();
        var manager = new Mock<IServiceModulesManager>();
        manager.SetupGet(m => m.ProcessExecutionService).Returns(processService.Object);

        processService
            .Setup(s => s.CancelAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new InvalidOperationException("Invalid status."));

        var sut = new ProcessesController(manager.Object);
        var response = await sut.Cancel(Guid.NewGuid());

        Assert.IsType<ConflictObjectResult>(response);
    }
}
