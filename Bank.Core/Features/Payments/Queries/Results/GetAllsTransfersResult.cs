namespace Bank.Core.Features.Payments.Queries.Results
{
    public class GetAllsTransfersResult : GetAllPaymentsByUsernameResult
    {
        public string UsernameSender { get; set; }
        public string ReceiverUsername { get; set; }
        public int AccountNumberSender { get; set; }
        public int AccountNumberReceiver { get; set; } 
    }
}
