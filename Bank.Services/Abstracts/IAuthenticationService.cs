using Bank.Data.Entities.Identity;
using Bank.Data.Helpers;

namespace Bank.Services.Abstracts
{
    public interface IAuthenticationService
    {
        Task<AuthModel> RegisterAsync(ApplicationUser model, string Password);

        Task<AuthModel> RegisterAsync(ApplicationUser model, string RoleName, string Password);

        Task<AuthModel> LoginAsync(string model, string Password);

        Task<string> ConfirmEmail(string Email, string code);

        Task<string> SendResetPasswordAsync(string Email);

        Task<string> ConfirmResetPasswordAsync(string Code, string Email);

        Task<string> ResetPasswordAsync(string Email, string Password);
    }
}
