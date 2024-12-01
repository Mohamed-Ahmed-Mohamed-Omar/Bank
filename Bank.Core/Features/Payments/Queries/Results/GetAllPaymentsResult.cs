namespace Bank.Core.Features.Payments.Queries.Results
{
    public class GetAllPaymentsResult : GetAllPaymentsByUsernameResult
    {
        public string Username { get; set; }
        public int Id {  get; set; } 
        public string Email { get; set; }
    }
}
