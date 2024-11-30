using Bank.Data.Entities;

namespace Bank.Services.Abstracts
{
    public interface IAccountServices
    {
        Task<Account> CreateAccounAsync(Account account, string username);
        Task<string> DeleteAccountAsync(int id);
        Task<IQueryable<object>> GetAllAccountsAsync();
        Task<object> GetAccountAsync(string userame);
        Task<object> GetAccountByIdAsync(string STRING);
        Task<Account> GetAccountByUsernameAsync(string username);
    }
}
