using Bank.Core.Bases;
using Bank.Core.Features.Admin.Commands.Models;
using Bank.Services.Abstracts;
using MediatR;

namespace Bank.Core.Features.Admin.Commands.Handlers
{
    public class RoleCommandHandler : IRequestHandler<AddRoleCommand, Response<string>>,
                                      IRequestHandler<DeleteRoleCommand, Response<string>>,
                                      IRequestHandler<DeleteUserCommand, Response<string>>,
                                      IRequestHandler<UpdateUserRolesCommand, Response<string>>
    {
        private readonly IAdminService _adminService;

        public RoleCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<Response<string>> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminService.AddRoleAsync(request.RoleName);

            if (result.Contains("already exists"))
            {
                return new Response<string>
                {
                    Success = false,
                    Message = result
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = result
            };
        }

        public async Task<Response<string>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminService.DeleteRoleAsync(request.RoleName);

            if (result.Contains("not found"))
            {
                return new Response<string>
                {
                    Success = false,
                    Message = result
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = result
            };
        }

        public async Task<Response<string>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            // Call the RemoveUserAsync method from the admin service
            var result = await _adminService.RemoveUserAsync(request.UserName);

            // Check the result and return the response
            if (!result.Done)
            {
                return new Response<string>
                {
                    Success = false,
                    Message = result.Message
                };
            }

            return new Response<string>
            {
                Success = true,
                Message = result.Message
            };
        }

        public async Task<Response<string>> Handle(UpdateUserRolesCommand request, CancellationToken cancellationToken)
        {
            var result = await _adminService.UpdateUserRolesAsync(request.UserName, request.RoleName);

            // Check the result and construct the response
            if (result == "Success")
            {
                return new Response<string>
                {
                    Success = true,
                    Message = "User roles updated successfully.",
                    Data = result
                };
            }
            else
            {
                return new Response<string>
                {
                    Success = false,
                    Message = result, // return the error message received from service
                    Data = null
                };
            }
        }
    }
}
