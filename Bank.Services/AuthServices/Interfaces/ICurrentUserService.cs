using Bank.Data.Entities.Identity;

namespace Bank.Services.AuthServices.Interfaces
{
    public interface ICurrentUserService
    {
        string GetUserNameAsync();

        Task<string> GetEmailByUsernameAsync(string username);

        Task<string> GetPhoneNumberByUsernameAsync(string username);

        Task<ApplicationUser> GetCurrentUserAsync();
    }
}
