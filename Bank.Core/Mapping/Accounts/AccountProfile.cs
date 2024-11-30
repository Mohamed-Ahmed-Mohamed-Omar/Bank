using AutoMapper;

namespace Bank.Core.Mapping.Accounts
{
    public partial class AccountProfile : Profile
    {
        public AccountProfile() 
        {
            AddAccountCommandMapping();
        }
    }
}
