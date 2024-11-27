using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Admin.Commands.Models
{
    public class UpdateUserRolesCommand : IRequest<Response<string>>
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }
    }
}
