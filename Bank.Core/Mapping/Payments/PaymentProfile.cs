using AutoMapper;

namespace Bank.Core.Mapping.Payments
{
    public partial class PaymentProfile : Profile
    {
        public PaymentProfile() 
        {
            PaymentMapping();
        }
    }
}
