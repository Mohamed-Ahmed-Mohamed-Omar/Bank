using Bank.Core.Features.Payments.Commands.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bank.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("ProcessPayment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentCommand paymentCommand)
        {
            if (paymentCommand == null)
            {
                return BadRequest("Invalid payment data.");
            }

            var response = await _mediator.Send(paymentCommand);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferCommand transferCommand)
        {
            if (transferCommand == null)
            {
                return BadRequest("Invalid transfer data.");
            }

            var response = await _mediator.Send(transferCommand);

            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response.Message);
        }

    }
}
