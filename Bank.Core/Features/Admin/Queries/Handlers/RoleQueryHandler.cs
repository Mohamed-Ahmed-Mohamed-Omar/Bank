using AutoMapper;
using Bank.Core.Bases;
using Bank.Core.Features.Admin.Queries.Models;
using Bank.Core.Features.Admin.Queries.Results;
using Bank.Core.Wrappers;
using Bank.Services.Abstracts;
using MediatR;

namespace Bank.Core.Features.Admin.Queries.Handlers
{
    public class RoleQueryHandler : IRequestHandler<GetRolesListQuery, Response<List<GetRolesListResult>>>,
                                    IRequestHandler<GetUsersByRoleNameListQuery, PaginatedResult<GetUsersByRoleNameListResult>>,
                                    IRequestHandler<GetUserPaginationQuery, PaginatedResult<GetUserPaginationReponse>>
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;

        public RoleQueryHandler(IAdminService adminService, IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }

        public async Task<Response<List<GetRolesListResult>>> Handle(GetRolesListQuery request, CancellationToken cancellationToken)
        {
            // Get all roles as IQueryable
            var roles = await _adminService.GetAllRolesAsync();

            // Project the data to GetRolesListResult
            var result = roles.Select(r => new GetRolesListResult
            {
                Name = r.Name
            }).ToList();

            return new Response<List<GetRolesListResult>>
            {
                Success = true,
                Message = "Roles retrieved successfully.",
                Data = result
            };
        }

        public async Task<PaginatedResult<GetUsersByRoleNameListResult>> Handle(GetUsersByRoleNameListQuery request, CancellationToken cancellationToken)
        {
            // Fetch users based on role
            var users = await _adminService.GetAllUserByRoleNameAsync(request.RoleName);

            // Use AutoMapper to map to GetUsersByRoleNameListResult
            var mappedUsers = _mapper.Map<List<GetUsersByRoleNameListResult>>(users);

            // Apply pagination
            IEnumerable<GetUsersByRoleNameListResult> usersQuery;

            // Check if we should use IQueryable or List based on the source
            if (mappedUsers is IQueryable<GetUsersByRoleNameListResult>)
            {
                usersQuery = mappedUsers.AsQueryable(); // Use AsQueryable if it's IQueryable
            }
            else
            {
                usersQuery = mappedUsers; // Use the List directly if it's a List
            }

            // Return paginated result
            return await usersQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);
        }

        public async Task<PaginatedResult<GetUserPaginationReponse>> Handle(GetUserPaginationQuery request, CancellationToken cancellationToken)
        {
            // Fetch all users
            var users = await _adminService.GetAllUsersAsync();

            // Map users to GetUserPaginationReponse
            var mappedUsers = _mapper.Map<List<GetUserPaginationReponse>>(users);

            // Determine whether to use IQueryable or List
            IEnumerable<GetUserPaginationReponse> usersQuery;

            if (mappedUsers is IQueryable<GetUserPaginationReponse>) // If mappedUsers is already IQueryable
            {
                usersQuery = mappedUsers.AsQueryable(); // Just use AsQueryable
            }
            else
            {
                usersQuery = mappedUsers; // If it's a List or IEnumerable, convert to IQueryable
            }

            // Get paginated result
            var paginatedResult = await usersQuery.ToPaginatedListAsync(request.PageNumber, request.PageSize);

            // Add role name to each user (if needed)
            foreach (var user in paginatedResult.Data)
            {
                var roleName = await _adminService.GetRoleNameByUserNameAsync(user.UserName);
                user.RoleName = roleName; // Set the role name for each user
            }

            return paginatedResult;
        }
    }
}
