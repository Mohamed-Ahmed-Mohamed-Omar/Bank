namespace Bank.Core.Features.Accounts.Queries.Results
{
    public class GetAccountByIdResponse : GetAccountByNameResponse
    {
        public string UserId { get; set; }
    }
}
