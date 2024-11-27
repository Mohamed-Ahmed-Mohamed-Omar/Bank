using Bank.Data.Entities.Identity;

namespace Bank.Services.AuthServices.Interfaces
{
    public interface ICurrentUserService
    {
        string GetUserId();
        string GetUserName();
        Task<ApplicationUser> GetCurrentUserAsync();
    }
}
