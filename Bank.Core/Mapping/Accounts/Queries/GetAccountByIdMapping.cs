using Bank.Core.Features.Accounts.Queries.Results;
using Bank.Data.Entities;

namespace Bank.Core.Mapping.Accounts
{
    public partial class AccountProfile
    {
        public void GetAccountByIdMapping()
        {
            CreateMap<Account, GetAccountByIdResponse>();
        }
    }
}
