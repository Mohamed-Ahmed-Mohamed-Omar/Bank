using Bank.Core.Features.Accounts.Commands.Models;
using Bank.Data.Entities;

namespace Bank.Core.Mapping.Accounts
{
    public partial class AccountProfile
    {
        public void AddAccountCommandMapping()
        {
            CreateMap<AddAccountCommand, Account>().ReverseMap();
        }
    }
}
