using Bank.Data.Entities;
using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrustructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> CreateAccounAsync(Account account, string userName)
        {
            try
            {
                if (string.IsNullOrEmpty(userName))
                    throw new ArgumentException("UserName cannot be null or empty.");

                if (account.Balance < 0)
                    throw new ArgumentException("Balance cannot be negative.");

                if (account.Balance < 100)
                    throw new ArgumentException("Balance must be at Greater Than 100.");

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

                return "Success";
            }
            catch (ArgumentException ex)
            {
                // Handle specific validation exceptions
                return $"Validation Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Handle all other types of exceptions
                return $"An error occurred: {ex.Message}";
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

        public async Task<Account> GetAccountAsync(string username)
        {
            var account = await _context.accounts
                .AsNoTracking() // Optional: Use for read-only operations
                .FirstOrDefaultAsync(s => s.UserName == username);

            return account;
        }

        public async Task<IQueryable<Account>> GetAllAccountsAsync()
        {
            var data = _context.accounts.AsNoTracking();

            return data;
        }

        public async Task<Account> GetAccountByIdAsync(string id)
        {
            int accountId;

            // Safely parse the id if it's numeric
            var query = _context.accounts.AsNoTracking()
                .Where(s => (int.TryParse(id, out accountId) && s.Id == accountId) || s.UserName == id || s.AccountNumber == id);

            // Get the first matching account
            var account = await query.FirstOrDefaultAsync();

            return account;
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
    }
}
