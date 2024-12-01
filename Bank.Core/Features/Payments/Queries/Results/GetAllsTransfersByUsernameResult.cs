namespace Bank.Core.Features.Payments.Queries.Results
{
    public class GetAllsTransfersByUsernameResult : GetAllPaymentsByUsernameResult
    {
        public int ReceiverAccountId { get; set; }
        public string ReceiverUsername { get; set; }
    }
}
