using Bank.Data.Entities.Identity;
using Bank.Services.AuthServices.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Bank.Services.AuthServices.Implementations
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

        public string GetUserNameAsync()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task<ApplicationUser> GetCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return null;  // or throw an exception if userId is not found

            // Return the full user object based on the userId
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<string> GetEmailByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty.");

            // Retrieve the user by username
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                throw new ArgumentException($"User with username '{username}' does not exist.");

            // Return the email
            return user.Email;
        }

        public async Task<string> GetPhoneNumberByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty.");

            // Retrieve the user by username
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
                throw new ArgumentException($"User with username '{username}' does not exist.");

            // Return the phone number
            return user.PhoneNumber;
        }
    }

}
