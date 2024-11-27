using Bank.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Bank.Infrustructure.Abstracts
{
    public interface IAdminRepository
    {
        Task<ApplicationUser> GetUserByIdAsync(string userId);
        Task<ApplicationUser> GetUserByNameAsync(string userName);
        Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName);
        Task<List<IdentityRole>> GetAllRolesAsync();
        Task AddUserToRoleAsync(ApplicationUser user, string roleName);
        Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName);
        Task RemoveUserAsync(ApplicationUser user);
        Task<IdentityRole> GetRoleByNameAsync(string roleName);
        Task AddRoleAsync(string roleName);
        Task DeleteRoleAsync(string roleName);
        Task<bool> CheckRoleExistsAsync(string roleName);
        Task<bool> CheckUserIsInRoleAsync(string userNameOrId, string roleName);
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<string> GetRoleNameByUserNameAsync(string userName);
        Task<string> UpdateUserRolesAsync(string username, string rolename);
    }
}
