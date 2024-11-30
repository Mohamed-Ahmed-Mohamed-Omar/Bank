using Bank.Data.Entities;
using Bank.Infrustructure.Abstracts;
using Bank.Services.Abstracts;

namespace Bank.Services.Implementations
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentServices(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<Payment> PaymentAsync(Payment paymentm, string username)
        {
            return await _paymentRepository.PaymentAsync(paymentm, username);
        }
    }
}
