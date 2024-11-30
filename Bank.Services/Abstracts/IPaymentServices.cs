using Bank.Data.Entities;

namespace Bank.Services.Abstracts
{
    public interface IPaymentServices
    {
        Task<Payment> PaymentAsync(Payment paymentm, string username);
    }
}
