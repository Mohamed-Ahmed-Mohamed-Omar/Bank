using Bank.Data.Entities;
using Bank.InfrastructureBases;
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
                    throw new ArgumentException("Balance must be at least 100.");

                // Create a new Account object
                Account newAccount = new Account
                {
                    UserId = userName,
                    AccountNumber = GenerateRandomString(),
                    Balance = account.Balance,
                    CreatedAt = DateTime.UtcNow,
                };

                // Add to database and save changes
                await _context.accounts.AddAsync(newAccount);
                await _context.SaveChangesAsync();

                return "Success";
            }
            catch (ArgumentException ex)
            {
                // Handle specific validation exceptions
                return $"Validation Error: {ex.Message}";
            }
            catch (DbUpdateException ex)
            {
                // Handle database update related exceptions (e.g., duplicate account number)
                return $"Database Error: {ex.Message}";
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

        public async Task<object> GetAccountAsync(string id)
        {
              var data = _context.accounts
                .Where(s =>s.Id == int.Parse(id) || s.UserId == id || s.AccountNumber == id) 
                .Select(s => new
    {
                    Id = s.Id,
                    AccountNumber = s.AccountNumber,
                    Balance = s.Balance,
                    CreatedAt = s.CreatedAt,
                    UserId = s.UserId,
                    Email = s.User.Email,
                    Phone = s.User.PhoneNumber
                });

            return data;
        }

        public async Task<IQueryable<object>> GetAllAccountsAsync()
        {
            var data = _context.accounts
                .Where(s => s != null) 
                .Select(s => new
                {
                    Id = s.Id,
                    AccountNumber = s.AccountNumber,
                    Balance = s.Balance,
                    CreatedAt = s.CreatedAt,
                    UserId = s.UserId,
                    Email = s.User.Email,
                    Phone = s.User.PhoneNumber
                });

            return data;
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
