using Bank.Data.Entities.Identity;
using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrustructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddRoleAsync(string roleName)
        {
            var role = new IdentityRole(roleName);

            _context.Roles.Add(role);

            await _context.SaveChangesAsync();
        }

        public async Task AddUserToRoleAsync(ApplicationUser user, string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null) throw new Exception("Role not found");

            var userRole = new IdentityUserRole<string>
            {
                UserId = user.Id,
                RoleId = role.Id
            };

            _context.UserRoles.Add(userRole);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckRoleExistsAsync(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

            return role != null;
        }

        public async Task<bool> CheckUserIsInRoleAsync(string userNameOrId, string roleName)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userNameOrId || u.Id == userNameOrId);
            if (user == null)
                return false;

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
                return false;

            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

            return userRole != null;
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

            if (role != null)
            {
                _context.Roles.Remove(role);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return _userManager.Users.ToList();  // Get all users from UserManager
        }

        public async Task<IdentityRole> GetRoleByNameAsync(string roleName)
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        }

        public async Task<string> GetRoleNameByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName); // Find user by username
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user); // Get roles of the user
            return roles.FirstOrDefault();
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string roleName)
        {
            // Fetch the role based on the roleName
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                return new List<ApplicationUser>(); // Return an empty list if the role does not exist
            }

            // Fetch users who are in the given role
            var userIdsInRole = await _context.UserRoles
                .Where(ur => ur.RoleId == role.Id)
                .Select(ur => ur.UserId) // Get the user IDs associated with this role
                .ToListAsync();

            // Fetch the users' details based on the user IDs
            var usersInRole = await _context.Users
                .Where(u => userIdsInRole.Contains(u.Id)) // Filter users who are in the role
                .Select(u => new ApplicationUser
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    CreditCardNumber = u.CreditCardNumber // Include additional fields like CreditCardNumber
                })
                .ToListAsync();

            return usersInRole;
        }

        public async Task RemoveUserAsync(ApplicationUser user)
        {
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromRoleAsync(ApplicationUser user, string roleName)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null) throw new Exception("Role not found");

            var userRole = await _context.UserRoles

                .FirstOrDefaultAsync(ur => ur.UserId == user.Id && ur.RoleId == role.Id);

            if (userRole != null)
            {
                _context.UserRoles.Remove(userRole);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> UpdateUserRolesAsync(string username, string rolename)
        {
            // Start a database transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Step 1: Get the user by username
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    return "UserNotFound";
                }

                // Step 2: Get the user's current roles
                var userRoles = await _userManager.GetRolesAsync(user);

                // Step 3: Remove the user's current roles
                if (userRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
                    if (!removeResult.Succeeded)
                    {
                        return "FailedToRemoveOldRoles";
                    }
                }

                // Step 4: Add the new role
                var addRoleResult = await _userManager.AddToRoleAsync(user, rolename);
                if (!addRoleResult.Succeeded)
                {
                    return "FailedToAddNewRole";
                }

                // Step 5: Commit the transaction
                await transaction.CommitAsync();

                // Return success message
                return "Success";
            }
            catch (Exception ex)
            {
                // Roll back the transaction on error
                await transaction.RollbackAsync();
                // Optionally log the exception (e.g., using a logging framework)
                return $"FailedToUpdateUserRoles: {ex.Message}";
            }
        }
    }
}
