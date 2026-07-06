using Contracts;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace BamboeUp.Api.Context
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public long UserId => long.TryParse(User?.FindFirstValue("UserId"), out var id) ? id : 0;

        public Guid UserGuid => Guid.TryParse(User?.FindFirstValue("UserGuid"), out var guid) ? guid : Guid.Empty;

        public string UserName => User?.Identity?.Name ?? string.Empty;

        public string Role => User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        public long? CompanyId => long.TryParse(User?.FindFirstValue("CompanyId"), out var id) ? id : null;

        public long? OfficeId => long.TryParse(User?.FindFirstValue("OfficeId"), out var id) ? id : null;

        public bool IsAdmin => Role == "Admin";
    }
}
