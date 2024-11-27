using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Accounts.Commands.Models
{
    public class DeleteAccountCommand : IRequest<Response<string>>
    {
        public int Id { get; set; }
    }
}
