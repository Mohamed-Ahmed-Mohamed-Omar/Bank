using Bank.Data.Entities;

namespace Bank.Infrustructure.Abstracts
{
    public interface IPaymentRepository
    {
        Task<Payment> PaymentAsync(Payment paymentm, string username);
        Task<Payment> TransferAsync(Payment payment, string username);
        Task<List<object>> GetAllPaymentsByUsernameAsync(string username);
        Task<List<object>> GetAllPaymentsAsync();
        Task<List<object>> GetAllsTransfersByUsernameAsync(string username);
        Task<List<object>> GetAllsTransfersAsync();
    }
}
