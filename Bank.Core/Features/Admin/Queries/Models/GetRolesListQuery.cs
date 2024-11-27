using Bank.Core.Bases;
using Bank.Core.Features.Admin.Queries.Results;
using MediatR;

namespace Bank.Core.Features.Admin.Queries.Models
{
    public class GetRolesListQuery : IRequest<Response<List<GetRolesListResult>>>
    {
    }
}
