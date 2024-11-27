using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Admin.Commands.Models
{
    public class DeleteRoleCommand : IRequest<Response<string>>
    {
        public string RoleName { get; set; }
    }
}
