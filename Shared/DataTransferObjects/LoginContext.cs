
namespace Shared.DataTransferObjects
{
    public class LoginContext
    {
        public UserDto User { get; set; } = new();
        public CompanyDto Company { get; set; } = new();
        public CompanyOfficeDto Office { get; set; } = new();

        /// <summary>The single person bound to this login. Self Service screens read PersonId from here.</summary>
        public PersonDto? Person { get; set; }
        public List<LoginScopeDto> AvailableScopes { get; set; } = new();

        /// <summary>
        /// UserGroupScopeId dari scope yang aktif saat login/switch.
        /// Digunakan NavMenu untuk match scope secara eksplisit (Gap 2 Fix).
        /// </summary>
        public long? ActiveUserGroupScopeId { get; set; }
    }
}
