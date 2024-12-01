using Bank.Data.Entities;

namespace Bank.Services.Abstracts
{
    public interface IPaymentServices
    {
        Task<Payment> PaymentAsync(Payment paymentm, string username);
        Task<Payment> TransferAsync(Payment payment, string username);
        Task<List<object>> GetAllPaymentsByUsernameAsync(string username);
        Task<List<object>> GetAllPaymentsAsync();
        Task<List<object>> GetAllsTransfersByUsernameAsync(string username);
        Task<List<object>> GetAllsTransfersAsync();
    }
}
