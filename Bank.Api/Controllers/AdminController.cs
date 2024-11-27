using Bank.Core.Features.Admin.Commands.Models;
using Bank.Core.Features.Admin.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Add Role
        [HttpPost("AddRole")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        // Delete Role
        [HttpDelete("DeleteRole/{roleName}")]
        public async Task<IActionResult> DeleteRole([FromRoute] string roleName)
        {
            var command = new DeleteRoleCommand { RoleName = roleName };
            var response = await _mediator.Send(command);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        // Delete User
        [HttpDelete("DeleteUser/{userName}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string userName)
        {
            var command = new DeleteUserCommand { UserName = userName };
            var response = await _mediator.Send(command);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        // Update User Roles
        [HttpPost("UpdateUserRoles")]
        public async Task<IActionResult> UpdateUserRoles([FromBody] UpdateUserRolesCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        // Get List of Roles
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetRolesList()
        {
            var query = new GetRolesListQuery();
            var response = await _mediator.Send(query);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        // Get Users By Role
        [HttpGet("GetUsersByRole/{roleName}")]
        public async Task<IActionResult> GetUsersByRole([FromRoute] string roleName, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetUsersByRoleNameListQuery
            {
                RoleName = roleName,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _mediator.Send(query);
            if (response.Succeeded)
                return Ok(response);
            return BadRequest(response);
        }

        // Get Users with Pagination
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetUserPaginationQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _mediator.Send(query);
            if (response.Succeeded)
                return Ok(response);
            return BadRequest(response);
        }
    }
}
