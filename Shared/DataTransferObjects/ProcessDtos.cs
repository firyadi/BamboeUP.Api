using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public class ProcessDefinitionDto
{
    public Guid ProcessDefinitionGuid { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Category { get; set; }
    public int Version { get; set; }
    public byte ExecutionMode { get; set; }
    public bool RequiresApproval { get; set; }
    public bool AllowRetry { get; set; }
    public bool AllowCancel { get; set; }
    public bool IsActive { get; set; }
    public List<ProcessParameterDefinitionDto> Parameters { get; set; } = [];
    public List<ProcessStepDefinitionDto> Steps { get; set; } = [];
}

public class ProcessParameterDefinitionDto
{
    public string ParameterCode { get; set; } = string.Empty;
    public string ParameterName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string ControlType { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? LookupSource { get; set; }
    public int DisplayOrder { get; set; }
    public string? GroupName { get; set; }
    public bool IsVisible { get; set; }
}

public class ProcessStepDefinitionDto
{
    public int StepNo { get; set; }
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string StepType { get; set; } = string.Empty;
    public bool IsMandatory { get; set; }
    public bool AllowRetry { get; set; }
    public int ExecutionOrder { get; set; }
}

public class ProcessParameterValueDto
{
    public string ParameterCode { get; set; } = string.Empty;
    public string? ValueText { get; set; }
    public decimal? ValueNumber { get; set; }
    public DateTime? ValueDate { get; set; }
    public string? ValueJson { get; set; }
}

public class ProcessExecuteRequestDto
{
    public List<ProcessParameterValueDto> Parameters { get; set; } = [];
    public long? CompanyId { get; set; }
    public long? CompanyOfficeId { get; set; }
    public Guid? CorrelationId { get; set; }
}

public class ProcessValidateRequestDto
{
    public List<ProcessParameterValueDto> Parameters { get; set; } = [];
}

public class ProcessValidationMessageDto
{
    public string Severity { get; set; } = "Error";
    public string Message { get; set; } = string.Empty;
    public string? ParameterCode { get; set; }
}

public class ProcessValidationResultDto
{
    public bool IsValid { get; set; }
    public List<ProcessValidationMessageDto> Messages { get; set; } = [];
}

public class ProcessExecutionDto
{
    public Guid ProcessExecutionGuid { get; set; }
    public Guid ProcessDefinitionGuid { get; set; }
    public string ProcessCode { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public string ExecutionNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal ProgressPercent { get; set; }
    public string? CurrentStepCode { get; set; }
    public int ErrorCount { get; set; }
    public int WarningCount { get; set; }
    public Guid CorrelationId { get; set; }
    public long? CompanyId { get; set; }
    public List<ProcessParameterValueDto> Parameters { get; set; } = [];
    public List<ProcessStepExecutionDto> Steps { get; set; } = [];
    public ProcessResultDto? Result { get; set; }
}

public class ProcessStepExecutionDto
{
    public string StepCode { get; set; } = string.Empty;
    public string StepName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public decimal ProgressPercent { get; set; }
    public int ProcessedCount { get; set; }
    public int SuccessCount { get; set; }
    public int WarningCount { get; set; }
    public int FailedCount { get; set; }
    public string? Message { get; set; }
    public int RetryCount { get; set; }
}

public class ProcessMessageDto
{
    public string? StepCode { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedTime { get; set; }
}

public class ProcessResultDto
{
    public string ResultStatus { get; set; } = string.Empty;
    public string? ResultMessage { get; set; }
    public int TotalRecords { get; set; }
    public int SuccessRecords { get; set; }
    public int WarningRecords { get; set; }
    public int FailedRecords { get; set; }
    public string? ResultReference { get; set; }
    public DateTime CompletedAt { get; set; }
}

public class ProcessResultDetailDto
{
    public string RecordKey { get; set; } = string.Empty;
    public string? RecordLabel { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
}

public class ProcessExecutionSearchDto
{
    public string? Status { get; set; }
    public string? ProcessCode { get; set; }
    public DateTime? StartedFrom { get; set; }
    public DateTime? StartedTo { get; set; }
    public long? CompanyId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public class ProcessRetryRequestDto
{
    public string RetryMode { get; set; } = "Process";
    public string? StepCode { get; set; }
}

public class ProcessApprovalRequestDto
{
    [Required]
    public string Comment { get; set; } = string.Empty;
}
