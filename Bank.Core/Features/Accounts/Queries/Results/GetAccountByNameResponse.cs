using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Core.Features.Accounts.Queries.Results
{
    public class GetAccountByNameResponse
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
