using Bank.Data.Entities;
using Bank.Data.Entities.Identity;
using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrustructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Account> CreateAccounAsync(Account account, string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    throw new ArgumentException("UserName cannot be null or empty.");

                if (account.Balance < 0)
                    throw new ArgumentException("Balance cannot be negative.");

                if (account.Balance < 100)
                    throw new ArgumentException("Balance must be greater than 100.");

                // Create a new Account object
                Account newAccount = new Account
                {
                    UserName = userName,
                    AccountNumber = GenerateRandomString(),
                    Balance = account.Balance,
                    CreatedAt = DateTime.UtcNow,
                };

                // Add to database and save changes
                _context.accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                return newAccount;
            }
            catch (ArgumentException ex)
            {
                // Log exception (optional) and rethrow it as a custom exception or handle it appropriately
                throw new ApplicationException($"Validation Error: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Log exception (optional) and rethrow it as a custom exception or handle it appropriately
                throw new ApplicationException($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        public async Task<string> DeleteAccountAsync(int id)
        {
            // Retrieve the account by ID
            var account = await _context.accounts.FindAsync(id);

            // Check if the account exists
            if (account == null)
            {
                return $"Account with ID {id} not found.";
            }

            // Delete the account from the database
            _context.accounts.Remove(account);
            await _context.SaveChangesAsync();

            return "Account deleted successfully.";
        }

        public async Task<object> GetAccountAsync(string username)
        {
            // Fetch the account record from the database
            var account = await _context.accounts
                .Where(s => s.UserName == username)
                .Select(s => new
                {
                    s.Id,
                    s.AccountNumber,
                    s.Balance,
                    s.CreatedAt,
                    s.UserName
                })
                .FirstOrDefaultAsync();

            // If the account is not found, return null
            if (account == null)
                return null;

            // Perform async operations to get Email and Phone
            var email = await GetEmailByUsernameAsync(account.UserName);
            var phone = await GetPhoneNumberByUsernameAsync(account.UserName);

            // Return the full object with all necessary information
            return new
            {
                account.AccountNumber,
                account.Balance,
                account.CreatedAt,
                Email = email,
                Phone = phone
            };
        }

        public async Task<IQueryable<object>> GetAllAccountsAsync()
        {
            // Fetch the accounts from the database without async operations
            var accounts = _context.accounts
                .Where(s => s != null)
                .Select(s => new
                {
                    s.Id,
                    s.AccountNumber,
                    s.Balance,
                    s.CreatedAt,
                    s.UserName
                }).AsQueryable();

            // Convert the result to a list to perform asynchronous operations
            var accountList = accounts.ToList();

            // Prepare a list of results after executing asynchronous operations
            var result = new List<object>();

            foreach (var account in accountList)
            {
                var email = await GetEmailByUsernameAsync(account.UserName);
                var phone = await GetPhoneNumberByUsernameAsync(account.UserName);

                result.Add(new
                {
                    account.Id,
                    account.AccountNumber,
                    account.Balance,
                    account.CreatedAt,
                    account.UserName,
                    Email = email,
                    Phone = phone
                });
            }

            // Convert list to IQueryable
            return result.AsQueryable();
        }

        public async Task<object> GetAccountByIdAsync(string id)
        {
            // Fetch the account record without async operations
            var account = await _context.accounts
                .Where(s => s.Id == int.Parse(id) || s.UserName == id || s.AccountNumber == id)
                .Select(s => new
                {
                    s.Id,
                    s.AccountNumber,
                    s.Balance,
                    s.CreatedAt,
                    UserName = s.UserName
                })
                .FirstOrDefaultAsync();

            // If the account is not found, we return null
            if (account == null)
                return null;

            // Perform async operations to get Email and Phone
            var email = await GetEmailByUsernameAsync(account.UserName);
            var phone = await GetPhoneNumberByUsernameAsync(account.UserName);

            // Return the full object
            return new
            {
                account.Id,
                account.AccountNumber,
                account.Balance,
                account.CreatedAt,
                UserName = account.UserName,
                Email = email,
                Phone = phone
            };
        }

        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            return await _context.accounts
                                  .FirstOrDefaultAsync(acc => acc.UserName == username);
        }

        private static string GenerateRandomString()
        {
            Random random = new Random();

            // Generate 4 random letters (a-z, A-Z)
            string letters = new string(Enumerable.Range(0, 4)
                .Select(_ => (char)(random.Next(0, 2) == 0
                    ? random.Next('a', 'z' + 1) // Lowercase letter
                    : random.Next('A', 'Z' + 1))) // Uppercase letter
                .ToArray());

            // Generate 4 random digits (0-9)
            string numbers = new string(Enumerable.Range(0, 4)
                .Select(_ => (char)random.Next('0', '9' + 1))
                .ToArray());

            // Combine and shuffle the result
            string combined = letters + numbers;
            string shuffled = new string(combined.OrderBy(_ => random.Next()).ToArray());

            return shuffled;
        }

        private async Task<string> GetEmailByUsernameAsync(string username)
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

        private async Task<string> GetPhoneNumberByUsernameAsync(string username)
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
