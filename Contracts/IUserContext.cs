namespace Contracts
{
    public interface IUserContext
    {
        long UserId { get; }
        Guid UserGuid { get; }
        string UserName { get; }
        string Role { get; }
        long? CompanyId { get; }
        long? OfficeId { get; }
        long? UserGroupScopeId { get; }
        bool IsAdmin { get; }
        bool IsAuthenticated { get; }
    }
}
