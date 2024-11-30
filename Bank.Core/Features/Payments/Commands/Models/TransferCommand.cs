using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Payments.Commands.Models
{
    public class TransferCommand : IRequest<Response<string>>
    {
        public decimal Amount { get; set; }

        public int ReceiverAccountNumber { get; set; }

        public string? Description { get; set; }

        public string PaymentMethod { get; set; } 
    }
}
