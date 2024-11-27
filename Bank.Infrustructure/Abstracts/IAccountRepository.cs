using Bank.Data.Entities;

namespace Bank.Infrustructure.Abstracts
{
    public interface IAccountRepository
    {
        Task<string> CreateAccounAsync(Account account, string UserName);
        Task<string> DeleteAccountAsync(int id);
        Task<IQueryable<object>> GetAllAccountsAsync();
        Task<object> GetAccountAsync(string id);
    }
}
