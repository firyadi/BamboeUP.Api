using System;

namespace Shared.DataTransferObjects
{
    public partial record EmployeeAssignmentDto
    {
        public string EmployeeAssignmentName => $"Assignment {EmployeeAssignmentId} {(string.IsNullOrWhiteSpace(Remarks) ? EffectiveFrom.ToString("dd-MM-yyyy") : Remarks)}";
    }
}
