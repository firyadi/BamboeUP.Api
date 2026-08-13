namespace Shared.DataTransferObjects
{
    public partial record EmployeeContractDto
    {
        public string EmployeeContractName => ContractNo;
    }
}
