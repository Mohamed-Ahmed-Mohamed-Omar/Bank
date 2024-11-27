using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Admin.Commands.Models
{
    public class DeleteUserCommand : IRequest<Response<string>>
    {
        public string UserName { get; set; }
    }
}
