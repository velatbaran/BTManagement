using BTManagement.Core;

namespace BTManagement.WebUI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirst("Username")?.Value;
    }
}
