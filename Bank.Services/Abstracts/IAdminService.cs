using Bank.Data.Entities.Identity;
using Bank.Data.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Bank.Services.Abstracts
{
    public interface IAdminService
    {
        Task<ResponseGeneral> AddUserToRoleAsync(string userNameOrId, string roleName);

        Task<ResponseGeneral> RemoveUserFromRoleAsync(string userNameOrId, string roleName);

        Task<List<ApplicationUser>> GetAllUserByRoleNameAsync(string roleName);

        Task<ResponseGeneral> RemoveUserAsync(string userNameOrId);

        Task<List<IdentityRole>> GetAllRolesAsync();

        Task<string> AddRoleAsync(string roleName);

        Task<string> DeleteRoleAsync(string roleName);

        Task<List<ApplicationUser>> GetAllUsersAsync();

        Task<string> GetRoleNameByUserNameAsync(string userName);

        Task<string> UpdateUserRolesAsync(string username, string rolename);
    }
}
