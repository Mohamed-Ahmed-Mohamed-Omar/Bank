using Bank.Data.Entities;
using Bank.InfrastructureBases;
using Bank.Infrustructure.Abstracts;
using Bank.Infrustructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Bank.Infrustructure.Repositories
{
    public class AccountRepository : GenericRepositoryAsync<Account>, IAccountRepository
    {
        private DbSet<Account> _accounts;

        public AccountRepository(ApplicationDbContext context) : base(context)
        {
            _accounts = context.Set<Account>();
        }
    }
}
