namespace Bank.Core.Features.Accounts.Queries.Results
{
    public class GetAccountByNameResponse
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
