using Bank.Data.Entities;
using Bank.InfrastructureBases;

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
