using Bank.Data.Entities.Identity;
using Bank.Services.AuthServices.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Bank.Api.AuthServices.Implementations
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public string GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public string GetUserName()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = GetUserId();
            if (userId == null) return null;

            return await _userManager.FindByIdAsync(userId);
        }
    }
}
