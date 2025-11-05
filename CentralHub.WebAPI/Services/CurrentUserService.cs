using CentralHub.Application.Interfaces;
using System.Security.Claims;

namespace CentralHub.WebAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Read the User ID (sub) claim from the token
        public Guid UserId
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(id, out var userId) ? userId : Guid.Empty;
            }
        }

        // Read custom "tenant_id" claim from the token
        public Guid TenantId
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User?.FindFirstValue("tenant_id");
                return Guid.TryParse(id, out var tenantId) ? tenantId : Guid.Empty;
            }
        }

        // Read the Email claim from the token
        public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }
}
