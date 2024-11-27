using Bank.Data.Entities;
using Bank.InfrastructureBases;

namespace Bank.Infrustructure.Abstracts
{
    public interface IAccountRepository : IGenericRepositoryAsync<Account>
    {
    }
}
