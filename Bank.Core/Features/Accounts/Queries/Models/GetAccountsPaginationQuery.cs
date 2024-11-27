using Bank.Core.Features.Accounts.Queries.Results;
using Bank.Core.Wrappers;
using MediatR;

namespace Bank.Core.Features.Accounts.Queries.Models
{
    public class GetAccountsPaginationQuery : IRequest<PaginatedResult<GetAccountsPaginationReponse>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
