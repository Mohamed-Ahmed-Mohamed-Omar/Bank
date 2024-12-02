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

        public Task<List<object>> GetAllPaymentsByUsernameAsync(string username)
        {
            return _paymentRepository.GetAllPaymentsByUsernameAsync(username);
        }

        public Task<List<object>> GetAllsTransfersByUsernameAsync(string username)
        {
            return _paymentRepository.GetAllsTransfersByUsernameAsync(username);
        }

        public async Task<List<object>> GetAllPaymentsAsync()
        {
            return await _paymentRepository.GetAllPaymentsAsync();
        }

        public Task<List<object>> GetAllsTransfersAsync()
        {
            return _paymentRepository.GetAllsTransfersAsync();
        }

        public async Task<Payment> PaymentAsync(Payment paymentm, string username)
        {
            return await _paymentRepository.PaymentAsync(paymentm, username);
        }

        public async Task<Payment> TransferAsync(Payment payment, string username)
        {
            return await _paymentRepository.TransferAsync(payment, username);
        }

        public async Task<object> GetPaymentDetailsAsync(string username, string PaymentMethod, string Description, string PaymentType, decimal Amount)
        {
            return await _paymentRepository.GetPaymentDetailsAsync(username, PaymentMethod, Description, PaymentType, Amount);
        }
    }
}
