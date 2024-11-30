using Bank.Core.Features.Payments.Commands.Models;
using Bank.Data.Entities;

namespace Bank.Core.Mapping.Payments
{
    public partial class PaymentProfile
    {
        public void TransferMapping()
        {
            CreateMap<TransferCommand, Payment>();
        }
    }
}
