using AutoMapper;

namespace Bank.Core.Mapping.Admin
{
    public partial class AdminProfile : Profile
    {
        public AdminProfile() 
        {
            GetUsersByRoleNameListMapping();
            GetUserPaginationMapping();
        }
    }
}
