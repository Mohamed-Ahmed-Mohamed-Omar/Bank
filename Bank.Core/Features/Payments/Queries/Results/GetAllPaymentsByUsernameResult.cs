namespace Bank.Core.Features.Payments.Queries.Results
{
    public class GetAllPaymentsByUsernameResult
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentType { get; set; }
        public string Status { get; set; }
        public int ReferenceNumber { get; set; }
    }
}
