using Bank.Core.Features.Payments.Commands.Models;
using Bank.Core.Features.Payments.Queries.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
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
        [Authorize]
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

        // Endpoint to get all payments by username
        [HttpGet("GetPaymentsByUsername")]
        public async Task<IActionResult> GetAllPaymentsByUsername([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllPaymentsByUsernameQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            if (result != null)
                return Ok(result);
            return BadRequest(result);
        }

        // Endpoint to get all payments
        [HttpGet("GetAllPayments")]
        public async Task<IActionResult> GetAllPayments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllPaymentsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            if (result != null)
                return Ok(result);
            return BadRequest(result);
        }

        // Endpoint to get all transfers by username
        [HttpGet("GetTransfersByUsername")]
        public async Task<IActionResult> GetAllTransfersByUsername([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllsTransfersByUsernameQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            if (result != null)
                return Ok(result);
            return BadRequest(result);
        }

        // Endpoint to get all transfers
        [HttpGet("GetAllTransfers")]
        public async Task<IActionResult> GetAllTransfers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllsTransfersQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            if (result != null)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
