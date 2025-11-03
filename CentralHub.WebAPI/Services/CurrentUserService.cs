using CentralHub.Application.Interfaces;

namespace CentralHub.WebAPI.Services
{
    // Placeholder implementation for ICurrentUserService
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // --- AUTHENTICATION NOT YET IMPLEMENTED ---
        public Guid TenantId => GetDemoTenantId();
        public Guid UserId => GetDemoUserId();
        public string? Email => "demo-user@centralhub.com";

        private Guid GetDemoTenantId()
        {
            // Placeholder logic, returns a hardcoded Guid for now
            return new Guid("11111111-1111-1111-1111-111111111111");
        }

        private Guid GetDemoUserId()
        {
            // Placeholder logic
            return new Guid("22222222-2222-2222-2222-222222222222");
        }
    }
}
