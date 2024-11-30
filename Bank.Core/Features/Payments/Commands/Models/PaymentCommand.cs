using Bank.Core.Bases;
using MediatR;

namespace Bank.Core.Features.Payments.Commands.Models
{
    public class PaymentCommand : IRequest<Response<string>>
    {
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
    }
}
