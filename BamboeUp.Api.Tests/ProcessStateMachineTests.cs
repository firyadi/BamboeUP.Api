using Service.Modules.Processes;
using Shared.Process;

namespace BamboeUp.Api.Tests;

public class ProcessStateMachineTests
{
    [Theory]
    [InlineData(ProcessExecutionStatus.Ready, ProcessExecutionStatus.Queued)]
    [InlineData(ProcessExecutionStatus.Queued, ProcessExecutionStatus.Running)]
    [InlineData(ProcessExecutionStatus.Running, ProcessExecutionStatus.Completed)]
    [InlineData(ProcessExecutionStatus.Running, ProcessExecutionStatus.CompletedWithWarning)]
    [InlineData(ProcessExecutionStatus.Running, ProcessExecutionStatus.Failed)]
    [InlineData(ProcessExecutionStatus.Failed, ProcessExecutionStatus.Queued)]
    [InlineData(ProcessExecutionStatus.CompletedWithWarning, ProcessExecutionStatus.Queued)]
    [InlineData(ProcessExecutionStatus.Running, ProcessExecutionStatus.Stopping)]
    [InlineData(ProcessExecutionStatus.Stopping, ProcessExecutionStatus.Cancelled)]
    public void CanTransition_AllowedPath_ReturnsTrue(string from, string to)
    {
        Assert.True(ProcessExecutionStateMachine.CanTransition(from, to));
    }

    [Theory]
    [InlineData(ProcessExecutionStatus.Completed, ProcessExecutionStatus.Queued)]
    [InlineData(ProcessExecutionStatus.Draft, ProcessExecutionStatus.Running)]
    [InlineData(ProcessExecutionStatus.Cancelled, ProcessExecutionStatus.Completed)]
    public void EnsureTransition_InvalidPath_Throws(string from, string to)
    {
        Assert.Throws<InvalidOperationException>(() =>
            ProcessExecutionStateMachine.EnsureTransition(from, to));
    }
}
