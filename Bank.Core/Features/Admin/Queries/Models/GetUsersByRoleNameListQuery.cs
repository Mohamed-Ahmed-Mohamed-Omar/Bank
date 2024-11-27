using Bank.Core.Features.Admin.Queries.Results;
using Bank.Core.Wrappers;
using MediatR;

namespace Bank.Core.Features.Admin.Queries.Models
{
    public class GetUsersByRoleNameListQuery : IRequest<PaginatedResult<GetUsersByRoleNameListResult>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string RoleName { get; set; }
    }
}
