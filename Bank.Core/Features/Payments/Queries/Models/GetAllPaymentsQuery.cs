using Bank.Core.Features.Payments.Queries.Results;
using Bank.Core.Wrappers;
using MediatR;

namespace Bank.Core.Features.Payments.Queries.Models
{
    public class GetAllPaymentsQuery : IRequest<PaginatedResult<GetAllPaymentsResult>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
