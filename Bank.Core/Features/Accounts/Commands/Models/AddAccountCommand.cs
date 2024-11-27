using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Accounts.Commands.Models
{
    public class AddAccountCommand : IRequest<Response<string>>
    {
        public decimal Balance { get; set; }
    }
}
