using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Admin.Commands.Models
{
    public class AddRoleCommand : IRequest<Response<string>>
    {
        public string RoleName { get; set; }
    }
}
