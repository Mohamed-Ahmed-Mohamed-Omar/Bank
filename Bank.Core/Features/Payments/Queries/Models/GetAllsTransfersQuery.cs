using Bank.Core.Features.Payments.Queries.Results;
using Bank.Core.Wrappers;
using MediatR;

namespace Bank.Core.Features.Payments.Queries.Models
{
    public class GetAllsTransfersQuery : IRequest<PaginatedResult<GetAllsTransfersResult>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
