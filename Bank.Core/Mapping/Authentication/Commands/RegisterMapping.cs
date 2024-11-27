using Bank.Core.Features.Authentication.Commands.Models;
using Bank.Data.Entities.Identity;

namespace Bank.Core.Mapping.Authentication
{
    public partial class AuthenticationProfile
    {
        public void RegisterMapping()
        {
            CreateMap<RegisterCommand, ApplicationUser>();
        }
    }
}
