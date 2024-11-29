using Bank.Data.Entities;
using Bank.Infrustructure.Abstracts;
using Bank.Services.Abstracts;

namespace Bank.Services.Implementations
{
    public class AccountServices : IAccountServices
    {
        private readonly IAccountRepository _accountRepository;

        public AccountServices(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<string> CreateAccounAsync(Account account, string username)
        {
            return await _accountRepository.CreateAccounAsync(account, username);
        }

        public async Task<string> DeleteAccountAsync(int id)
        {
            return await _accountRepository.DeleteAccountAsync(id);
        }

        public async Task<Account> GetAccountAsync(string id)
        {
            return await _accountRepository.GetAccountAsync(id);
        }

        public async Task<Account> GetAccountByIdAsync(string username)
        {
            return await _accountRepository.GetAccountByIdAsync(username);
        }

        public async Task<IQueryable<Account>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAccountsAsync();
        }
    }
}
