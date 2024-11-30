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

        public async Task<Account> CreateAccounAsync(Account account, string username)
        {
            return await _accountRepository.CreateAccounAsync(account, username);
        }

        public async Task<string> DeleteAccountAsync(int id)
        {
            return await _accountRepository.DeleteAccountAsync(id);
        }

        public async Task<object> GetAccountAsync(string username)
        {
            return await _accountRepository.GetAccountAsync(username);
        }

        public async Task<Account> GetAccountByAccountNumberAsync(string accountNumber)
        {
            return await _accountRepository.GetAccountByAccountNumberAsync(accountNumber);
        }

        public async Task<object> GetAccountByIdAsync(string username)
        {
            return await _accountRepository.GetAccountByIdAsync(username);
        }

        public async Task<Account> GetAccountByUsernameAsync(string username)
        {
            return await _accountRepository.GetAccountByUsernameAsync(username);
        }

        public async Task<IQueryable<object>> GetAllAccountsAsync()
        {
            return await _accountRepository.GetAllAccountsAsync();
        }

        public Task UpdateAccountAsync(Account account)
        {
            return _accountRepository.UpdateAccountAsync(account);
        }
    }
}
