using Bank.Core.Bases;
using Bank.Core.Features.Accounts.Queries.Results;
using MediatR;

namespace Bank.Core.Features.Accounts.Queries.Models
{
    public class GetAccountByNameQuery : IRequest<Response<GetAccountByNameResponse>>
    {
    }
}
