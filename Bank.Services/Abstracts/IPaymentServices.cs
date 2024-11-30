using Bank.Data.Entities;

namespace Bank.Services.Abstracts
{
    public interface IPaymentServices
    {
        Task<Payment> PaymentAsync(Payment paymentm, string username);
        Task<Payment> TransferAsync(Payment payment, string username);
    }
}
