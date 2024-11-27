using Bank.Core.Features.Authentication.Commands.Models;
using Bank.Core.Features.Authentication.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterCommand command)
        {
            var response = await _mediator.Send(command);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpPost("RegisterWithRole")]
        public async Task<IActionResult> RegisterWithRole([FromForm] RegisterWithRoleCommand command)
        {
            var response = await _mediator.Send(command);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            if (!response.Success)
                return Unauthorized(response);

            return Ok(response);
        }


        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordCommand command)
        {
            var response = await _mediator.Send(command);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpPost("SendResetPassword")]
        public async Task<IActionResult> SendResetPassword([FromForm] SendResetPasswordCommand command)
        {
            var response = await _mediator.Send(command);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailQuery query)
        {
            var response = await _mediator.Send(query);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }


        [HttpGet("ConfirmResetPassword")]
        public async Task<IActionResult> ConfirmResetPassword([FromQuery] ConfirmResetPasswordQuery query)
        {
            var response = await _mediator.Send(query);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
