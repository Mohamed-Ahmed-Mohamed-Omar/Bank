using Bank.Core.Features.Admin.Queries.Results;
using Bank.Data.Entities.Identity;

namespace Bank.Core.Mapping.Admin
{
    public partial class AdminProfile
    {
        public void GetUserPaginationMapping()
        {
            CreateMap<ApplicationUser, GetUserPaginationReponse>();
        }
    }
}
