using Bank.Data.Entities;

namespace Bank.Services.Abstracts
{
    public interface IAccountServices
    {
        Task<string> CreateAccounAsync(Account account, string username);
        Task<string> DeleteAccountAsync(int id);
        Task<IQueryable<Account>> GetAllAccountsAsync();
        Task<Account> GetAccountAsync(string id);
        Task<Account> GetAccountByIdAsync(string STRING);
    }
}
