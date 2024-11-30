using Bank.Data.Entities;

namespace Bank.Infrustructure.Abstracts
{
    public interface IAccountRepository
    {
        Task<Account> CreateAccounAsync(Account account, string UserName);
        Task<string> DeleteAccountAsync(int id);
        Task<IQueryable<object>> GetAllAccountsAsync();
        Task<object> GetAccountAsync(string username);
        Task<object> GetAccountByIdAsync(string id);
        Task<Account> GetAccountByUsernameAsync(string username);
        Task<Account> GetAccountByAccountNumberAsync(string accountNumber);
        Task UpdateAccountAsync(Account account);
    }
}
