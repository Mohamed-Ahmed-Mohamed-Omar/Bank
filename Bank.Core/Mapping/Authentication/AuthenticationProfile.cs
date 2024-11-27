using AutoMapper;

namespace Bank.Core.Mapping.Authentication
{
    public partial class AuthenticationProfile : Profile
    {
        public AuthenticationProfile() 
        {
            RegisterWithRoleMapping();
            RegisterMapping();
        }
    }
}
