using Bank.Core.Features.Accounts.Commands.Models;
using Bank.Core.Features.Accounts.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Add Account
        [HttpPost("AddAccount")]
        [Authorize]
        public async Task<IActionResult> AddAccount([FromBody] AddAccountCommand command)
        {
            var response = await _mediator.Send(command);
            if (response.Success)
            {
                // Account created successfully
                return Ok(response);
            }
            else if (response.RedirectUrl != null)
            {
                // Redirect to the existing account details page if account exists
                return Redirect(response.RedirectUrl); // Redirect to the URL passed in the response
            }

            return BadRequest(response);
        }

        // Delete Account
        [HttpDelete("DeleteAccount/{id}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] int id)
        {
            var command = new DeleteAccountCommand { Id = id };
            var response = await _mediator.Send(command);
            if (response.Success)
                return Ok(response);
            return BadRequest(response);
        }

        // Get Accounts with Pagination
        [HttpGet("GetAccounts")]
        public async Task<IActionResult> GetAccounts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAccountsPaginationQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var response = await _mediator.Send(query);
            if (response != null)
                return Ok(response);
            return BadRequest("Failed to fetch accounts.");
        }

        // Get Account by Name
        [HttpGet("GetAccountByName")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetAccountByName()
        {
            var query = new GetAccountByNameQuery();
            var response = await _mediator.Send(query);
            if (response.Success)
                return Ok(response);
            return NotFound(response);
        }

        // Get Account by ID
        [HttpGet("GetAccountById/{id}")]
        public async Task<IActionResult> GetAccountById([FromRoute] string id)
        {
            var query = new GetAccountByIdQuery { Id = id };
            var response = await _mediator.Send(query);
            if (response.Success)
                return Ok(response);
            return NotFound(response);
        }
    }
}
