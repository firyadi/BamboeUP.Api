
namespace Shared.DataTransferObjects
{
    public class LoginContext
    {
        public UserDto User { get; set; } = new();
        public CompanyDto Company { get; set; } = new();
        public CompanyOfficeDto Office { get; set; } = new();
        public List<LoginScopeDto> AvailableScopes { get; set; } = new();

        /// <summary>
        /// UserGroupScopeId dari scope yang aktif saat login/switch.
        /// Digunakan NavMenu untuk match scope secara eksplisit (Gap 2 Fix).
        /// </summary>
        public long? ActiveUserGroupScopeId { get; set; }
    }
}
