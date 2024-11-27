using Bank.Data.Entities.Identity;
using Bank.Data.Helpers;
using Bank.Infrustructure.Abstracts;
using Bank.Services.Abstracts;
using Microsoft.AspNetCore.Identity;

namespace Bank.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<ResponseGeneral> AddUserToRoleAsync(string userNameOrId, string roleName)
        {
            // Check if the user exists by either userName or userId
            var user = await _adminRepository.GetUserByNameAsync(userNameOrId) ??
                       await _adminRepository.GetUserByIdAsync(userNameOrId);

            if (user == null)
                return new ResponseGeneral { Done = false, Message = $"User with UserNameOrID: '{userNameOrId}' not found." };

            // Check if the role exists
            var roleExists = await _adminRepository.CheckRoleExistsAsync(roleName);
            if (!roleExists)
                return new ResponseGeneral { Done = false, Message = $"Role '{roleName}' not found." };

            // Add the user to the role
            await _adminRepository.AddUserToRoleAsync(user, roleName);
            return new ResponseGeneral { Done = true, Message = "User added to role successfully" };
        }

        public async Task<ResponseGeneral> RemoveUserFromRoleAsync(string userNameOrId, string roleName)
        {
            // Check if the user exists by either userName or userId
            var user = await _adminRepository.GetUserByNameAsync(userNameOrId) ??
                       await _adminRepository.GetUserByIdAsync(userNameOrId);

            if (user == null)
                return new ResponseGeneral { Done = false, Message = $"User with UserNameOrID: '{userNameOrId}' not found." };

            // Check if the user is assigned to the role
            var isUserInRole = await _adminRepository.CheckUserIsInRoleAsync(userNameOrId, roleName);
            if (!isUserInRole)
                return new ResponseGeneral { Done = false, Message = $"User is not in role '{roleName}'." };

            // Remove the user from the role
            await _adminRepository.RemoveUserFromRoleAsync(user, roleName);
            return new ResponseGeneral { Done = true, Message = "User removed from role successfully" };
        }

        public async Task<List<ApplicationUser>> GetAllUserByRoleNameAsync(string roleName)
        {
            // Check if the role exists
            var roleExists = await _adminRepository.CheckRoleExistsAsync(roleName);
            if (!roleExists)
            {
                // Instead of returning a list with a message, return an empty list or handle the error appropriately
                return new List<ApplicationUser>(); // Return an empty list if the role doesn't exist
            }

            // Get all users by role
            var users = await _adminRepository.GetUsersByRoleAsync(roleName);

            // Select only the needed properties
            return users.Select(u => new ApplicationUser
            {
                UserName = u.UserName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                CreditCardNumber = u.CreditCardNumber
            }).ToList();
        }

        public async Task<ResponseGeneral> RemoveUserAsync(string userNameOrId)
        {
            // Check if the user exists by either userName or userId
            var user = await _adminRepository.GetUserByNameAsync(userNameOrId) ??
                       await _adminRepository.GetUserByIdAsync(userNameOrId);

            if (user == null)
                return new ResponseGeneral { Done = false, Message = $"User with UserNameOrID: '{userNameOrId}' not found." };

            // Remove the user
            await _adminRepository.RemoveUserAsync(user);
            return new ResponseGeneral { Done = true, Message = "User removed successfully" };
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            // Get all roles
            var roles = await _adminRepository.GetAllRolesAsync();
            return roles;
        }

        public async Task<string> AddRoleAsync(string roleName)
        {
            // Check if the role already exists
            var roleExists = await _adminRepository.CheckRoleExistsAsync(roleName);
            if (roleExists)
                return $"Role '{roleName}' already exists.";

            // Add the new role
            await _adminRepository.AddRoleAsync(roleName);
            return $"Role '{roleName}' added successfully.";
        }

        public async Task<string> DeleteRoleAsync(string roleName)
        {
            // Check if the role exists
            var roleExists = await _adminRepository.CheckRoleExistsAsync(roleName);
            if (!roleExists)
                return $"Role '{roleName}' not found.";

            // Delete the role
            await _adminRepository.DeleteRoleAsync(roleName);
            return $"Role '{roleName}' deleted successfully.";
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            var data = await _adminRepository.GetAllUsersAsync();

            return data;
        }

        public async Task<string> GetRoleNameByUserNameAsync(string userName)
        {
            var data = await _adminRepository.GetRoleNameByUserNameAsync(userName);

            return data;
        }

        public async Task<string> UpdateUserRolesAsync(string username, string rolename)
        {
            var data = await _adminRepository.UpdateUserRolesAsync(username, rolename);

            return data;
        }
    }
}
